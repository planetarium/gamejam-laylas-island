using System.Numerics;
using Libplanet;
using Libplanet.Assets;
using LibUnity.Backend.State;
using LibUnity.Frontend.State.Modifiers;

namespace LibUnity.Frontend.State
{
    /// <summary>
    /// This is a static class that collects the patterns of using the `Add` and `Remove` functions of `LocalStateSettings`.
    /// </summary>
    public static class LocalLayerModifier
    {
        #region Agent, Avatar / Currency

        /// <summary>
        /// Modify the agent's gold.
        /// </summary>
        /// <param name="agentAddress"></param>
        /// <param name="gold"></param>
        public static void ModifyAgentGold(Address agentAddress, FungibleAssetValue gold)
        {
            if (gold.Sign == 0)
            {
                return;
            }

            var modifier = new AgentGoldModifier(gold);
            LocalLayer.Instance.Add(agentAddress, modifier);

            //FIXME Avoid LocalLayer duplicate modify gold.
            var state = new GoldBalanceState(agentAddress, Game.Instance.Agent.GetBalance(agentAddress, gold.Currency));
            if (!state.Address.Equals(agentAddress))
            {
                return;
            }

            States.Instance.SetGoldBalanceState(state);
        }

        public static void ModifyAgentGold(Address agentAddress, BigInteger gold)
        {
            if (gold == 0)
            {
                return;
            }

            ModifyAgentGold(agentAddress, new FungibleAssetValue(
                States.Instance.GoldBalanceState.Gold.Currency,
                gold,
                0));
        }

        #endregion
    }
}
