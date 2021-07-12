using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blocks;
using Libplanet.Tx;
using System;
using System.Linq;
using Lib9c;
using Libplanet;
using NCAction = Libplanet.Action.PolymorphicAction<LibUnity.Backend.Action.BaseAction>;

namespace LibUnity.Backend
{
    public class BlockPolicy : BlockPolicy<NCAction>
    {
        private readonly long _minimumDifficulty;
        private readonly long _difficultyBoundDivisor;

        /// <summary>
        /// Whether to ignore or respect hardcoded block indices to make older
        /// blocks compatible with the latest rules.  If it's turned off
        /// (by default) older blocks pass some new rules by force.
        /// Therefore, on the mainnet this should be turned off.
        /// This option is made mainly due to unit tests.  Turning on this
        /// option can be useful for tests.
        /// </summary>
        internal readonly bool IgnoreHardcodedIndicesForBackwardCompatibility;

        public BlockPolicy(
            IAction blockAction,
            TimeSpan blockInterval,
            long minimumDifficulty,
            int difficultyBoundDivisor,
            int maxTransactionsPerBlock,
            int maxBlockBytes,
            int maxGenesisBytes,
            Func<Transaction<NCAction>, BlockChain<NCAction>, bool> doesTransactionFollowPolicy = null
        )
            : this(
                blockAction: blockAction,
                blockInterval: blockInterval,
                minimumDifficulty: minimumDifficulty,
                difficultyBoundDivisor: difficultyBoundDivisor,
                maxTransactionsPerBlock: maxTransactionsPerBlock,
                maxBlockBytes: maxBlockBytes,
                maxGenesisBytes: maxGenesisBytes,
                ignoreHardcodedIndicesForBackwardCompatibility: false,
                doesTransactionFollowPolicy: doesTransactionFollowPolicy
            )
        {
        }

        internal BlockPolicy(
            IAction blockAction,
            TimeSpan blockInterval,
            long minimumDifficulty,
            int difficultyBoundDivisor,
            int maxTransactionsPerBlock,
            int maxBlockBytes,
            int maxGenesisBytes,
            bool ignoreHardcodedIndicesForBackwardCompatibility,
            Func<Transaction<NCAction>, BlockChain<NCAction>, bool> doesTransactionFollowPolicy = null
        )
            : base(
                blockAction,
                blockInterval,
                minimumDifficulty,
                difficultyBoundDivisor,
                maxTransactionsPerBlock,
                maxBlockBytes,
                maxGenesisBytes,
                doesTransactionFollowPolicy
            )
        {
            _minimumDifficulty = minimumDifficulty;
            _difficultyBoundDivisor = difficultyBoundDivisor;
            IgnoreHardcodedIndicesForBackwardCompatibility =
                ignoreHardcodedIndicesForBackwardCompatibility;
        }

        public override InvalidBlockException ValidateNextBlock(
            BlockChain<NCAction> blocks,
            Block<NCAction> nextBlock
        ) =>
            ValidateBlock(nextBlock)
            ?? ValidateMinerAuthority(nextBlock)
            ?? base.ValidateNextBlock(blocks, nextBlock);

        private InvalidBlockException ValidateBlock(Block<NCAction> block)
        {
            if (!(block.Miner is Address miner))
            {
                return null;
            }

            // To prevent selfish mining, we define a consensus that blocks with no transactions are do not accepted. 
            // (For backward compatibility, blocks before 1,711,631th don't have to be proven.
            // Note that as of Jun 16, 2021, there are about 1,710,000+ blocks.)
            if (block.Transactions.Count <= 0 &&
                (IgnoreHardcodedIndicesForBackwardCompatibility || block.Index > 1_711_631))
            {
                return new InvalidMinerException(
                    $"The block #{block.Index} {block.Hash} (mined by {miner}) should " +
                    "include at least one transaction.",
                    miner
                );
            }

            return null;
        }

        private InvalidBlockException ValidateMinerAuthority(Block<NCAction> block)
        {
            if (!(block.Miner is Address miner))
            {
                return null;
            }

            // Authority should be proven through a no-op transaction (= txs with zero actions).
            // (For backward compatibility, blocks before 1,200,000th don't have to be proven.
            // Note that as of Feb 9, 2021, there are about 770,000+ blocks.)
            Transaction<NCAction>[] txs = block.Transactions.ToArray();
            if (!txs.Any(tx => tx.Signer.Equals(miner) && !tx.Actions.Any()) &&
                block.ProtocolVersion > 0 &&
                (IgnoreHardcodedIndicesForBackwardCompatibility || block.Index > 1_200_000))
            {
#if DEBUG
                string debug =
                    "  Note that there " +
                    (txs.Length == 1 ? "is a transaction:" : $"are {txs.Length} transactions:") +
                    txs.Select((tx, i) =>
                            $"\n    {i}. {tx.Actions.Count} actions; signed by {tx.Signer}")
                        .Aggregate(string.Empty, (a, b) => a + b);
#else
                const string debug = "";
#endif
                return new InvalidMinerException(
                    $"The block #{block.Index} {block.Hash}'s miner {miner} should be proven by " +
                    "including a no-op transaction by signed the same authority." + debug,
                    miner
                );
            }

            return null;
        }
    }
}
