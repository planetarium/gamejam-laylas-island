using System;
using System.Collections.Generic;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Tx;
using Libplanet.Blockchain.Renderers;
using LibUnity.Backend.Renderer;
using Serilog;
using Serilog.Events;
using NCAction = Libplanet.Action.PolymorphicAction<LibUnity.Backend.Action.BaseAction>;

namespace LibUnity.Backend
{
    public class BlockPolicySource
    {
        public const int DifficultyBoundDivisor = 2048;

        // Note: The heaviest block of 9c-main (except for the genesis) weighs 58,408 B (58 KiB).
        public const int MaxBlockBytes = 1024 * 100; // 100 KiB

        // Note: The genesis block of 9c-main net weighs 11,085,640 B (11 MiB).
        public const int MaxGenesisBytes = 1024 * 1024 * 15; // 15 MiB

        private readonly TimeSpan _blockInterval = TimeSpan.FromSeconds(8);

        public readonly ActionRenderer ActionRenderer = new ActionRenderer();

        public readonly BlockRenderer BlockRenderer = new BlockRenderer();

        public readonly LoggedActionRenderer<NCAction> LoggedActionRenderer;

        public readonly LoggedRenderer<NCAction> LoggedBlockRenderer;

        public BlockPolicySource(ILogger logger, LogEventLevel logEventLevel = LogEventLevel.Verbose)
        {
            LoggedActionRenderer =
                new LoggedActionRenderer<NCAction>(ActionRenderer, logger, logEventLevel);

            LoggedBlockRenderer =
                new LoggedRenderer<NCAction>(BlockRenderer, logger, logEventLevel);
        }

        public IBlockPolicy<NCAction> GetPolicy(int minimumDifficulty, int maximumTransactions) =>
            GetPolicy(
                minimumDifficulty,
                maximumTransactions,
                ignoreHardcodedIndicesForBackwardCompatibility: false
            );

        // FIXME 남은 설정들도 설정화 해야 할지도?
        internal IBlockPolicy<NCAction> GetPolicy(
            int minimumDifficulty,
            int maximumTransactions,
            bool ignoreHardcodedIndicesForBackwardCompatibility
        )
        {
#if UNITY_EDITOR
            return new DebugPolicy();
#else
            return new BlockPolicy(
                new RewardGold(),
                blockInterval: _blockInterval,
                minimumDifficulty: minimumDifficulty,
                difficultyBoundDivisor: DifficultyBoundDivisor,
                maxTransactionsPerBlock: maximumTransactions,
                maxBlockBytes: MaxBlockBytes,
                maxGenesisBytes: MaxGenesisBytes,
                ignoreHardcodedIndicesForBackwardCompatibility: ignoreHardcodedIndicesForBackwardCompatibility,
                doesTransactionFollowPolicy: DoesTransactionFollowPolicy
            );
#endif
        }

        public IEnumerable<IRenderer<NCAction>> GetRenderers() =>
            new IRenderer<NCAction>[] {BlockRenderer, LoggedActionRenderer};

        private bool DoesTransactionFollowPolicy(
            Transaction<NCAction> transaction,
            BlockChain<NCAction> blockChain
        )
        {
            return
                transaction.Actions.Count <= 1;
        }
    }
}
