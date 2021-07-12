using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Tx;
using LibUnity.Backend.Action;
using Serilog;

namespace LibUnity.Backend
{
    public class Miner
    {
        private readonly BlockChain<PolymorphicAction<BaseAction>> _chain;
        private readonly Swarm<PolymorphicAction<BaseAction>> _swarm;
        private readonly PrivateKey _privateKey;

        public bool AuthorizedMiner { get; }

        public Address Address => _privateKey.ToAddress();

        public Transaction<PolymorphicAction<BaseAction>> StageProofTransaction()
        {
            // We assume authorized miners create no transactions at all except for
            // proof transactions.  Without the assumption, nonces for proof txs become
            // much complicated to determine.
            var proof = Transaction<PolymorphicAction<BaseAction>>.Create(
                _chain.GetNextTxNonce(_privateKey.ToAddress()),
                _privateKey,
                _chain.Genesis.Hash,
                new PolymorphicAction<BaseAction>[0]
            );
            _chain.StageTransaction(proof);
            return proof;
        }

        public async Task<Block<PolymorphicAction<BaseAction>>> MineBlockAsync(
            int maxTransactions,
            CancellationToken cancellationToken)
        {
            var txs = new HashSet<Transaction<PolymorphicAction<BaseAction>>>();
            var invalidTxs = txs;

            Block<PolymorphicAction<BaseAction>> block = null;
            try
            {
                if (AuthorizedMiner)
                {
                    _chain
                        .GetStagedTransactionIds()
                        .Select(txid => _chain.GetTransaction(txid)).ToList()
                        .ForEach(tx => _chain.UnstageTransaction(tx));
                    StageProofTransaction();
                }

                block = await _chain.MineBlock(
                    Address,
                    DateTimeOffset.UtcNow,
                    cancellationToken: cancellationToken,
                    maxTransactions: maxTransactions,
                    append: false);

                _chain.Append(block);
                if (_swarm is Swarm<PolymorphicAction<BaseAction>> s && s.Running)
                {
                    s.BroadcastBlock(block);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Mining was canceled due to change of tip.");
            }
            catch (InvalidTxException invalidTxException)
            {
                var invalidTx = _chain.GetTransaction(invalidTxException.TxId);

                Log.Debug($"Tx[{invalidTxException.TxId}] is invalid. mark to unstage.");
                invalidTxs.Add(invalidTx);
            }
            catch (UnexpectedlyTerminatedActionException actionException)
            {
                if (actionException.TxId is TxId txId)
                {
                    Log.Debug(
                        $"Tx[{actionException.TxId}]'s action is invalid. mark to unstage. {actionException}");
                    invalidTxs.Add(_chain.GetTransaction(txId));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"exception was thrown. {ex}");
            }
            finally
            {
#pragma warning disable LAA1002
                foreach (var invalidTx in invalidTxs)
#pragma warning restore LAA1002
                {
                    _chain.UnstageTransaction(invalidTx);
                }
            }

            return block;
        }

        public Miner(
            BlockChain<PolymorphicAction<BaseAction>> chain,
            Swarm<PolymorphicAction<BaseAction>> swarm,
            PrivateKey privateKey,
            bool authorizedMiner
        )
        {
            _chain = chain ?? throw new ArgumentNullException(nameof(chain));
            _swarm = swarm;
            _privateKey = privateKey;
            AuthorizedMiner = authorizedMiner;
        }
    }
}
