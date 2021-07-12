using System;
using Bencodex.Types;
using Libplanet;
using Libplanet.Blocks;
using Libplanet.Assets;
using Libplanet.Crypto;
using Libplanet.Tx;
using LibUnity.Backend.Action;
using LibUnity.Backend.Renderer;
using UniRx;

namespace LibUnity.Frontend.BlockChain
{
    public interface IAgent
    {
        Subject<long> BlockIndexSubject { get; }

        long BlockIndex { get; }

        PrivateKey PrivateKey { get; }

        Address Address { get; }

        BlockRenderer BlockRenderer { get; }

        ActionRenderer ActionRenderer { get; }

        int AppProtocolVersion { get; }

        Subject<BlockHash> BlockTipHashSubject { get; }

        BlockHash BlockTipHash { get; }

        void Initialize(
            CommandLineOptions options,
            PrivateKey privateKey,
            Action<bool> callback
        );

        void EnqueueAction(GameAction gameAction);

        IValue GetState(Address address);

        void SendException(Exception exc);

        bool IsActionStaged(Guid actionId, out TxId txId);

        FungibleAssetValue GetBalance(Address address, Currency currency);
    }
}
