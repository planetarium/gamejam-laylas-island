﻿using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Intentionally causes repetitive reorgs for debugging purpose.
    /// </summary>
    public class ReorgMiner
    {
        private readonly BlockChain<PolymorphicAction<BaseAction>> _mainChain;
        private readonly BlockChain<PolymorphicAction<BaseAction>> _subChain;
        private readonly Swarm<PolymorphicAction<BaseAction>> _mainSwarm;
        private readonly Swarm<PolymorphicAction<BaseAction>> _subSwarm;
        private readonly int _reorgInterval;

        public Address Address { get; }

        public async Task<(
                Block<PolymorphicAction<BaseAction>> MainBlock,
                Block<PolymorphicAction<BaseAction>> SubBlock)>
            MineBlockAsync(
                int maxTransactions,
                CancellationToken cancellationToken)
        {
            var txs = new HashSet<Transaction<PolymorphicAction<BaseAction>>>();

            var invalidTxs = txs;
            Block<PolymorphicAction<BaseAction>> mainBlock = null;
            Block<PolymorphicAction<BaseAction>> subBlock = null;
            try
            {
                mainBlock = await _mainChain.MineBlock(
                    Address,
                    DateTimeOffset.UtcNow,
                    cancellationToken: cancellationToken,
                    maxTransactions: maxTransactions);

                subBlock = await _subChain.MineBlock(
                    Address,
                    DateTimeOffset.UtcNow,
                    cancellationToken: cancellationToken,
                    maxTransactions: maxTransactions);

                if (_reorgInterval != 0 && subBlock.Index % _reorgInterval == 0)
                {
                    Log.Debug("Force reorg!");
                    _subSwarm.BroadcastBlock(subBlock);
                }

                if (_mainSwarm.Running)
                {
                    _mainSwarm.BroadcastBlock(mainBlock);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Mining was canceled due to change of tip.");
            }
            catch (InvalidTxException invalidTxException)
            {
                var invalidTx = _mainChain.GetTransaction(invalidTxException.TxId);

                Log.Debug($"Tx[{invalidTxException.TxId}] is invalid. mark to unstage.");
                invalidTxs.Add(invalidTx);
            }
            catch (UnexpectedlyTerminatedActionException actionException)
            {
                if (actionException.TxId is TxId txId)
                {
                    Log.Debug(
                        $"Tx[{actionException.TxId}]'s action is invalid. mark to unstage. {actionException}");
                    invalidTxs.Add(_mainChain.GetTransaction(txId));
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
                    _mainChain.UnstageTransaction(invalidTx);
                    _subChain.UnstageTransaction(invalidTx);
                }
            }

            return (mainBlock, subBlock);
        }

        public ReorgMiner(
            Swarm<PolymorphicAction<BaseAction>> mainSwarm,
            Swarm<PolymorphicAction<BaseAction>> subSwarm,
            PrivateKey privateKey,
            int reorgInterval)
        {
            _mainSwarm = mainSwarm ?? throw new ArgumentNullException(nameof(mainSwarm));
            _subSwarm = subSwarm ?? throw new ArgumentNullException(nameof(subSwarm));
            _mainChain = mainSwarm.BlockChain ?? throw new ArgumentNullException(nameof(mainSwarm.BlockChain));
            _subChain = subSwarm.BlockChain ?? throw new ArgumentNullException(nameof(subSwarm.BlockChain));
            Address = privateKey.PublicKey.ToAddress();
            _reorgInterval = reorgInterval;
        }
    }
}
