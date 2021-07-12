﻿using System;
using LaylasIsland.Backend.State;

namespace LaylasIsland.Frontend.State.Modifiers
{
    [Serializable]
    public abstract class AgentStateModifier : IAccumulatableStateModifier<AgentState>
    {
        public bool dirty { get; set; }
        public abstract bool IsEmpty { get; }
        public abstract void Add(IAccumulatableStateModifier<AgentState> modifier);
        public abstract void Remove(IAccumulatableStateModifier<AgentState> modifier);
        public abstract AgentState Modify(AgentState state);
    }
}
