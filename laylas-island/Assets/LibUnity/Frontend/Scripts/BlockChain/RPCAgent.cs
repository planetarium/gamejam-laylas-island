using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Bencodex;
using Bencodex.Types;
using Grpc.Core;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Tx;
using LibUnity.Backend;
using LibUnity.Backend.Action;
using LibUnity.Backend.Renderer;
using LibUnity.Backend.State;
using LibUnity.Frontend.State;
using LibUnity.RPC.Exceptions;
using LibUnity.RPC.Hubs;
using LibUnity.RPC.Services;
using MagicOnion.Client;
using UnityEngine;
using Logger = Serilog.Core.Logger;

namespace LibUnity.Frontend.BlockChain
{
    using UniRx;

    public class RPCAgent : MonoBehaviour, IAgent, IActionEvaluationHubReceiver
    {
        private const float TxProcessInterval = 1.0f;

        private readonly ConcurrentQueue<PolymorphicAction<BaseAction>> _queuedActions =
            new ConcurrentQueue<PolymorphicAction<BaseAction>>();

        private readonly TransactionMap _transactions = new TransactionMap(20);

        private Channel _channel;

        private IActionEvaluationHub _hub;

        private IBlockChainService _service;

        private Codec _codec = new Codec();

        private Block<PolymorphicAction<BaseAction>> _genesis;

        private DateTimeOffset _lastTipChangedAt;

        // Rendering logs will be recorded in NineChronicles.Standalone
        public BlockPolicySource BlockPolicySource { get; } = new BlockPolicySource(Logger.None);

        public BlockRenderer BlockRenderer => BlockPolicySource.BlockRenderer;

        public ActionRenderer ActionRenderer => BlockPolicySource.ActionRenderer;

        public Subject<long> BlockIndexSubject { get; } = new Subject<long>();

        public Subject<BlockHash> BlockTipHashSubject { get; } = new Subject<BlockHash>();

        public long BlockIndex { get; private set; }

        public PrivateKey PrivateKey { get; private set; }

        public Address Address => PrivateKey.PublicKey.ToAddress();

        public bool Connected { get; private set; }

        public readonly Subject<RPCAgent> OnDisconnected = new Subject<RPCAgent>();

        public readonly Subject<RPCAgent> OnRetryStarted = new Subject<RPCAgent>();

        public readonly Subject<RPCAgent> OnRetryEnded = new Subject<RPCAgent>();

        public readonly Subject<RPCAgent> OnPreloadStarted = new Subject<RPCAgent>();

        public readonly Subject<RPCAgent> OnPreloadEnded = new Subject<RPCAgent>();

        public int AppProtocolVersion { get; private set; }

        public BlockHash BlockTipHash { get; private set; }

        public void Initialize(
            CommandLineOptions options,
            PrivateKey privateKey,
            Action<bool> callback)
        {
            PrivateKey = privateKey;

            _channel = new Channel(
                options.RpcServerHost,
                options.RpcServerPort,
                ChannelCredentials.Insecure,
                new[]
                {
                    new ChannelOption("grpc.max_receive_message_length", -1)
                }
            );
            _lastTipChangedAt = DateTimeOffset.UtcNow;
            _hub = StreamingHubClient.Connect<IActionEvaluationHub, IActionEvaluationHubReceiver>(_channel, this);
            _service = MagicOnionClient.Create<IBlockChainService>(_channel);
            OnRenderBlock(null, _service.GetTip().ResponseAsync.Result);

            _genesis = BlockManager.ImportBlock(options.GenesisBlockPath ?? BlockManager.GenesisBlockPath);
            var appProtocolVersion = options.AppProtocolVersion is null
                ? default
                : Libplanet.Net.AppProtocolVersion.FromToken(options.AppProtocolVersion);
            AppProtocolVersion = appProtocolVersion.Version;

            RegisterDisconnectEvent(_hub);
            StartCoroutine(CoTxProcessor());
            StartCoroutine(CoJoin(callback));
        }

        public IValue GetState(Address address)
        {
            byte[] raw = _service.GetState(address.ToByteArray()).ResponseAsync.Result;
            return _codec.Decode(raw);
        }

        public FungibleAssetValue GetBalance(Address address, Currency currency)
        {
            var result = _service.GetBalance(
                address.ToByteArray(),
                _codec.Encode(currency.Serialize())
            );
            byte[] raw = result.ResponseAsync.Result;
            var serialized = (Bencodex.Types.List) _codec.Decode(raw);
            return FungibleAssetValue.FromRawValue(
                new Currency(serialized.ElementAt(0)),
                serialized.ElementAt(1).ToBigInteger());
        }

        public void SendException(Exception exc)
        {
        }

        public void EnqueueAction(GameAction action)
        {
            _queuedActions.Enqueue(action);
        }

        #region Mono

        private async void OnDestroy()
        {
            BlockRenderHandler.Instance.Stop();
            ActionRenderHandler.Instance.Stop();
            ActionUnrenderHandler.Instance.Stop();

            StopAllCoroutines();
            if (!(_hub is null))
            {
                await _hub.DisposeAsync();
            }

            if (!(_channel is null))
            {
                await _channel?.ShutdownAsync();
            }
        }

        #endregion

        private IEnumerator CoJoin(Action<bool> callback)
        {
            Task t = Task.Run(async () => { await _hub.JoinAsync(); });

            yield return new WaitUntil(() => t.IsCompleted);

            if (t.IsFaulted)
            {
                callback?.Invoke(false);
                yield break;
            }

            Connected = true;

            Currency goldCurrency = new GoldCurrencyState(
                (Dictionary) GetState(Addresses.GoldCurrency)
            ).Currency;
            States.Instance.SetAgentState(
                GetState(Address) is Bencodex.Types.Dictionary agentDict
                    ? new AgentState(agentDict)
                    : new AgentState(Address));
            States.Instance.SetGoldBalanceState(
                new GoldBalanceState(Address, GetBalance(Address, goldCurrency)));

            ActionRenderHandler.Instance.GoldCurrency = goldCurrency;
            BlockRenderHandler.Instance.Start(BlockRenderer);
            ActionRenderHandler.Instance.Start(ActionRenderer);
            ActionUnrenderHandler.Instance.Start(ActionRenderer);

            UpdateSubscribeAddresses();
            callback?.Invoke(true);
        }

        private IEnumerator CoTxProcessor()
        {
            while (true)
            {
                yield return new WaitForSeconds(TxProcessInterval);

                if (!_queuedActions.TryDequeue(out PolymorphicAction<BaseAction> action))
                {
                    continue;
                }

                Task task = Task.Run(async () =>
                {
                    await MakeTransaction(new List<PolymorphicAction<BaseAction>> {action});
                });
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                    // FIXME: Should restore this after fixing actual bug that MakeTransaction
                    // was throwing Exception.
                    /*Debug.LogError(
                        $"Unexpected exception occurred. re-enqueue {action} for retransmission."
                    );

                    _queuedActions.Enqueue(action);*/
                }
            }
        }

        private async Task MakeTransaction(List<PolymorphicAction<BaseAction>> actions)
        {
            long nonce = await GetNonceAsync();
            Transaction<PolymorphicAction<BaseAction>> tx =
                Transaction<PolymorphicAction<BaseAction>>.Create(
                    nonce,
                    PrivateKey,
                    _genesis?.Hash,
                    actions
                );
            await _service.PutTransaction(tx.Serialize(true));

            foreach (var action in actions)
            {
                var ga = (GameAction) action.InnerAction;
                _transactions.TryAdd(ga.Id, tx.Id);
            }
        }

        private async Task<long> GetNonceAsync()
        {
            return await _service.GetNextTxNonce(Address.ToByteArray());
        }

        public void OnRender(byte[] evaluation)
        {
            var formatter = new BinaryFormatter();
            using (var compressed = new MemoryStream(evaluation))
            using (var decompressed = new MemoryStream())
            using (var df = new DeflateStream(compressed, CompressionMode.Decompress))
            {
                df.CopyTo(decompressed);
                decompressed.Seek(0, SeekOrigin.Begin);
                var ev = (BaseAction.ActionEvaluation<BaseAction>) formatter.Deserialize(decompressed);
                ActionRenderer.ActionRenderSubject.OnNext(ev);
            }
        }

        public void OnUnrender(byte[] evaluation)
        {
            var formatter = new BinaryFormatter();
            using (var compressed = new MemoryStream(evaluation))
            using (var decompressed = new MemoryStream())
            using (var df = new DeflateStream(compressed, CompressionMode.Decompress))
            {
                df.CopyTo(decompressed);
                decompressed.Seek(0, SeekOrigin.Begin);
                var ev = (BaseAction.ActionEvaluation<BaseAction>) formatter.Deserialize(decompressed);
                ActionRenderer.ActionUnrenderSubject.OnNext(ev);
            }
        }

        public void OnRenderBlock(byte[] oldTip, byte[] newTip)
        {
            var newTipHeader = BlockHeader.Deserialize(newTip);
            BlockIndex = newTipHeader.Index;
            BlockIndexSubject.OnNext(BlockIndex);
            BlockTipHash = new BlockHash(newTipHeader.Hash);
            BlockTipHashSubject.OnNext(BlockTipHash);
            _lastTipChangedAt = DateTimeOffset.UtcNow;
            BlockRenderer.RenderBlock(null, null);
        }

        private async void RegisterDisconnectEvent(IActionEvaluationHub hub)
        {
            try
            {
                await hub.WaitForDisconnect();
            }
            finally
            {
                RetryRpc();
            }
        }

        private async void RetryRpc()
        {
            OnRetryStarted.OnNext(this);
            var retryCount = 10;
            Debug.Log($"Retry rpc connection. (count: {retryCount})");
            while (retryCount > 0)
            {
                await Task.Delay(5000);
                _hub = StreamingHubClient.Connect<IActionEvaluationHub, IActionEvaluationHubReceiver>(_channel, this);
                try
                {
                    Debug.Log($"Trying to join hub...");
                    await _hub.JoinAsync();
                    Debug.Log($"Join complete! Registering disconnect event...");
                    RegisterDisconnectEvent(_hub);
                    UpdateSubscribeAddresses();
                    OnRetryEnded.OnNext(this);
                    return;
                }
                catch (RpcException re)
                {
                    Debug.LogWarning($"RpcException occurred. Retrying... {retryCount}\n{re}");
                    retryCount--;
                }
                catch (ObjectDisposedException ode)
                {
                    Debug.LogWarning($"ObjectDisposedException occurred. Retrying... {retryCount}\n{ode}");
                    retryCount--;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Unexpected error occurred during rpc connection. {e}");
                    break;
                }
            }

            Connected = false;
            OnDisconnected.OnNext(this);
        }

        public void OnReorged(byte[] oldTip, byte[] newTip, byte[] branchpoint)
        {
            BlockRenderer.RenderReorg(null, null, null);
        }

        public void OnReorgEnd(byte[] oldTip, byte[] newTip, byte[] branchpoint)
        {
            BlockRenderer.RenderReorgEnd(null, null, null);
        }

        public void OnException(int code, string message)
        {
            var key = "ERROR_UNHANDLED";
            var errorCode = "100";
            switch (code)
            {
                case (int) RPCException.NetworkException:
                    key = "ERROR_NETWORK";
                    errorCode = "101";
                    break;

                case (int) RPCException.InvalidRenderException:
                    key = "ERROR_INVALID_RENDER";
                    errorCode = "102";
                    break;
            }

            Debug.LogError($"RPCAgent exception occured: ${key}, ${errorCode}");
        }

        public void OnPreloadStart()
        {
            OnPreloadStarted.OnNext(this);
            Debug.Log($"On Preload Start");
        }

        public void OnPreloadEnd()
        {
            OnPreloadEnded.OnNext(this);
            Debug.Log($"On Preload End");
        }

        public void UpdateSubscribeAddresses()
        {
            var addresses = new List<Address> {Address};

            Debug.Log($"Subscribing addresses: {string.Join(", ", addresses)}");
            _service.SetAddressesToSubscribe(addresses.Select(addr => addr.ToByteArray()));
        }

        public bool IsActionStaged(Guid actionId, out TxId txId)
        {
            return _transactions.TryGetValue(actionId, out txId)
                   && _service.IsTransactionStaged(txId.ToByteArray()).ResponseAsync.Result;
        }
    }
}
