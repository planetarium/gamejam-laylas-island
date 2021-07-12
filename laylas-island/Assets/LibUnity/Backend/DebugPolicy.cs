using System;
using System.Collections.Generic;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blocks;
using Libplanet.Tx;
using LibUnity.Backend.Action;

namespace LibUnity.Backend
{
    public class DebugPolicy : IBlockPolicy<PolymorphicAction<BaseAction>>
    {
        public IComparer<BlockPerception> CanonicalChainComparer { get; } =
            new TotalDifficultyComparer(TimeSpan.FromSeconds(3));

        public IAction BlockAction { get; } = new RewardGold();

        public InvalidBlockException ValidateNextBlock(
            BlockChain<PolymorphicAction<BaseAction>> blocks,
            Block<PolymorphicAction<BaseAction>> nextBlock
        )
        {
            return null;
        }

        public long GetNextBlockDifficulty(BlockChain<PolymorphicAction<BaseAction>> blocks)
        {
            return blocks.Tip is null ? 0 : 1;
        }

        public int MaxTransactionsPerBlock => int.MaxValue;

        public int GetMaxBlockBytes(long index) => int.MaxValue;

        public bool DoesTransactionFollowsPolicy(
            Transaction<PolymorphicAction<BaseAction>> transaction,
            BlockChain<PolymorphicAction<BaseAction>> blockChain
        ) =>
            true;
    }
}
