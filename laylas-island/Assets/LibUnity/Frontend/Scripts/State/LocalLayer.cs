﻿using System;
using System.Collections.Generic;
using System.Linq;
using Libplanet;
using LibUnity.Backend.State;
using LibUnity.Frontend.State.Modifiers;
using UnityEngine;

namespace LibUnity.Frontend.State
{
    // todo: 기간이 지난 주간 랭킹 상태 변경자들을 자동으로 클리어해줘야 함.
    /// <summary>
    /// 체인이 포함하는 특정 상태에 대한 상태 변경자를 관리한다.
    /// 모든 상태 변경자는 대상 상태의 체인 내 주소를 기준으로 분류한다.
    /// </summary>
    public class LocalLayer
    {
        /// <summary>
        /// 변경자 정보는 대상 주소(Address), 상태 변경자(Modifiers)로 구성된다.
        /// </summary>
        /// <typeparam name="T">AgentStateModifier 등</typeparam>
        private class ModifierInfo<T> where T : class
        {
            public readonly Address Address;
            public readonly List<T> Modifiers;

            public ModifierInfo(Address address)
            {
                Address = address;
                Modifiers = new List<T>();
            }
        }

        public static LocalLayer Instance => Game.Instance.LocalLayer;

        private ModifierInfo<AgentStateModifier> _agentModifierInfo;

        private ModifierInfo<AgentGoldModifier> _agentGoldModifierInfo;

        #region Initialization

        /// <summary>
        /// 인자로 받은 에이전트 상태를 바탕으로 로컬 세팅을 초기화 한다.
        /// 에이전트 상태가 포함하는 모든 아바타 상태 또한 포함된다.
        /// 이미 초기화되어 있는 에이전트와 같을 경우에는 아바타의 주소에 대해서만
        /// </summary>
        /// <param name="agentState"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void InitializeAgent(AgentState agentState)
        {
            if (agentState is null)
            {
                throw new ArgumentNullException(nameof(agentState));
            }

            var address = agentState.Address;
            // 이미 초기화되어 있는 에이전트와 같을 경우.
            if (!(_agentModifierInfo is null) &&
                _agentModifierInfo.Address.Equals(address))
            {
                return;
            }

            // _agentModifierInfo 초기화하기.
            _agentModifierInfo =
                new ModifierInfo<AgentStateModifier>(address);
            _agentGoldModifierInfo = new ModifierInfo<AgentGoldModifier>(address);
        }

        #endregion

        #region Add

        /// <summary>
        /// 인자로 받은 에이전트에 대한 상태 변경자를 더한다.
        /// </summary>
        /// <param name="agentAddress"></param>
        /// <param name="modifier"></param>
        public void Add(Address agentAddress, AgentStateModifier modifier)
        {
            // FIXME: 다른 Add() 오버로드와 겹치는 로직이 아주 많음.
            if (modifier is null ||
                modifier.IsEmpty)
            {
                return;
            }

            var modifierInfo = _agentModifierInfo;
            if (agentAddress.Equals(modifierInfo.Address))
            {
                var modifiers = modifierInfo.Modifiers;
                if (TryGetSameTypeModifier(modifier, modifiers, out var outModifier))
                {
                    outModifier.Add(modifier);
                    if (outModifier.IsEmpty)
                    {
                        modifiers.Remove(outModifier);
                    }
                }
                else
                {
                    modifiers.Add(modifier);
                }
            }
        }

        public void Add(Address agentAddress, AgentGoldModifier modifier)
        {
            // FIXME: 다른 Add() 오버로드와 겹치는 로직이 아주 많음.
            if (modifier is null || modifier.IsEmpty)
            {
                return;
            }

            if (agentAddress.Equals(_agentGoldModifierInfo.Address))
            {
                var modifiers = _agentGoldModifierInfo.Modifiers;
                if (TryGetSameTypeModifier(modifier, modifiers, out var outModifier))
                {
                    outModifier.Add(modifier);
                    if (outModifier.IsEmpty)
                    {
                        modifiers.Remove(outModifier);
                    }
                }
                else
                {
                    modifiers.Add(modifier);
                }
            }
        }

        #endregion

        #region Remove

        /// <summary>
        /// 인자로 받은 에이전트에 대한 상태 변경자를 뺀다.
        /// </summary>
        /// <param name="agentAddress"></param>
        /// <param name="modifier"></param>
        public void Remove(Address agentAddress, AgentStateModifier modifier)
        {
            if (modifier is null ||
                modifier.IsEmpty)
            {
                return;
            }

            var modifierInfo = _agentModifierInfo;
            if (agentAddress.Equals(modifierInfo.Address))
            {
                var modifiers = modifierInfo.Modifiers;
                if (TryGetSameTypeModifier(modifier, modifiers, out var outModifier))
                {
                    outModifier.Remove(modifier);
                    if (outModifier.IsEmpty)
                    {
                        modifiers.Remove(outModifier);
                    }

                    modifier = outModifier;
                }
                else
                {
                    modifier = null;
                }
            }

            if (modifier is null)
            {
                Debug.LogWarning(
                    $"[{nameof(LocalLayer)}] No found {nameof(modifier)} of {nameof(agentAddress)}");
            }
        }

        #endregion

        #region Modify

        /// <summary>
        /// 인자로 받은 에이전트 상태에 로컬 세팅을 반영한다.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public AgentState Modify(AgentState state)
        {
            if (state is null ||
                !state.Address.Equals(_agentModifierInfo.Address))
            {
                return state;
            }

            return PostModify(state, _agentModifierInfo);
        }

        /// <summary>
        /// 인자로 받은 잔고 상태에 로컬 세팅을 반영한다.
        /// </summary>
        public GoldBalanceState Modify(GoldBalanceState state)
        {
            if (state is null ||
                !state.Address.Equals(_agentGoldModifierInfo.Address))
            {
                return state;
            }

            return PostModify(state, _agentGoldModifierInfo);
        }

        private static TState PostModify<TState, TModifier>(
            TState state,
            ModifierInfo<TModifier> modifierInfo)
            where TState : BaseState
            where TModifier : class, IStateModifier<TState>
        {
            foreach (var modifier in modifierInfo.Modifiers)
            {
                state = modifier.Modify(state);
            }

            return state;
        }

        #endregion

        /// <summary>
        /// `modifiers`가 `modifier`와 같은 타입의 객체를 포함하고 있다면, 그것을 반환한다.
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="modifiers"></param>
        /// <param name="outModifier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static bool TryGetSameTypeModifier<T>(
            T modifier,
            IEnumerable<T> modifiers,
            out T outModifier)
        {
            return TryGetSameTypeModifier(
                modifier.GetType(),
                modifiers,
                out outModifier);
        }

        private static bool TryGetSameTypeModifier<T>(
            Type type,
            IEnumerable<T> modifiers,
            out T outModifier)
        {
            try
            {
                outModifier = modifiers.First(e => e.GetType() == type);
                return true;
            }
            catch
            {
                outModifier = default;
                return false;
            }
        }
    }
}
