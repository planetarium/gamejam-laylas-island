using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using LaylasIsland.Backend.State;

namespace LaylasIsland.Backend.Action
{
    [Serializable]
    [ActionType("create_item")]

    public class CreateItem : GameAction
    {
        public override IAccountStateDelta Execute(IActionContext context)
        {
            var agentAddress = context.Signer;
            var states = context.PreviousStates;
            if (context.Rehearsal)
            {
                return states.SetState(agentAddress, MarkChanged);
            }

            if (!(states.GetState(agentAddress) is null))
            {
                throw new Exception($"{agentAddress.ToHex()} already exists");
            }

            return states.SetState(agentAddress, new AgentState(agentAddress).Serialize());
        }

        protected override IImmutableDictionary<string, IValue> PlainValueInternal => new Dictionary<string, IValue>
        {
        }.ToImmutableDictionary();
        protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
        {
        }
    }
}
