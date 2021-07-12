using Libplanet.Assets;
using LibUnity.Backend.Action;
using LibUnity.Backend.Action.Exceptions;
using LibUnity.Backend.State;
using LibUnity.Frontend.State;
using UnityEngine;

namespace LibUnity.Frontend.BlockChain
{
    public abstract class ActionHandler
    {
        public bool Pending;
        public Currency GoldCurrency { get; internal set; }

        protected bool ValidateEvaluationForAgentState<T>(BaseAction.ActionEvaluation<T> evaluation)
            where T : BaseAction
        {
            if (States.Instance.AgentState is null)
            {
                return false;
            }

            return evaluation.OutputStates.UpdatedAddresses.Contains(States.Instance.AgentState.Address);
        }

        protected bool HasUpdatedAssetsForCurrentAgent<T>(BaseAction.ActionEvaluation<T> evaluation)
            where T : BaseAction
        {
            if (States.Instance.AgentState is null)
            {
                return false;
            }

            return evaluation.OutputStates.UpdatedFungibleAssets.ContainsKey(States.Instance.AgentState.Address);
        }

        protected bool ValidateEvaluationForCurrentAgent<T>(BaseAction.ActionEvaluation<T> evaluation)
            where T : BaseAction
        {
            return !(States.Instance.AgentState is null) &&
                   evaluation.Signer.Equals(States.Instance.AgentState.Address);
        }

        protected AgentState GetAgentState<T>(BaseAction.ActionEvaluation<T> evaluation) where T : BaseAction
        {
            var agentAddress = States.Instance.AgentState.Address;
            return evaluation.OutputStates.GetAgentState(agentAddress);
        }

        protected GoldBalanceState GetGoldBalanceState<T>(BaseAction.ActionEvaluation<T> evaluation)
            where T : BaseAction
        {
            var agentAddress = States.Instance.AgentState.Address;
            return evaluation.OutputStates.GetGoldBalanceState(agentAddress, GoldCurrency);
        }

        protected void UpdateAgentState<T>(BaseAction.ActionEvaluation<T> evaluation) where T : BaseAction
        {
            Debug.LogFormat("Called UpdateAgentState<{0}>. Updated Addresses : `{1}`", evaluation.Action,
                string.Join(",", evaluation.OutputStates.UpdatedAddresses));
            UpdateAgentState(GetAgentState(evaluation));
            try
            {
                UpdateGoldBalanceState(GetGoldBalanceState(evaluation));
            }
            catch (BalanceDoesNotExistsException)
            {
                UpdateGoldBalanceState(null);
            }
        }

        private static void UpdateAgentState(AgentState state)
        {
            States.Instance.SetAgentState(state);
        }

        private static void UpdateGoldBalanceState(GoldBalanceState goldBalanceState)
        {
            States.Instance.SetGoldBalanceState(goldBalanceState);
        }
    }
}
