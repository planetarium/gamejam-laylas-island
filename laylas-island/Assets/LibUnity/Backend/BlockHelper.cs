using System;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Blocks;
using Libplanet.Crypto;
using LibUnity.Backend.Action;
using LibUnity.Backend.BlockChain;
using LibUnity.Backend.State;
using Serilog;

namespace LibUnity.Backend
{
    public static class BlockHelper
    {
        public static Block<PolymorphicAction<BaseAction>> MineGenesisBlock(
            GoldDistribution[] goldDistributions,
            int maximumTransactions = 100,
            PrivateKey privateKey = null,
            DateTimeOffset? timestamp = null
        )
        {
            privateKey ??= new PrivateKey();

            var currency = new Currency("Gold", 2, privateKey.ToAddress());
            var initialStatesAction = new InitializeStates
            (
                new GoldCurrencyState(currency),
                goldDistributions
            );
            var actions = new PolymorphicAction<BaseAction>[]
            {
                initialStatesAction,
            };
            var blockAction = new BlockPolicySource(Log.Logger)
                .GetPolicy(5000000, maximumTransactions)
                .BlockAction;
            return
                BlockChain<PolymorphicAction<BaseAction>>.MakeGenesisBlock(
                    actions,
                    privateKey,
                    blockAction: blockAction,
                    timestamp: timestamp);
        }
    }
}
