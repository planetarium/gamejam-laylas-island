using LibUnity.Backend.State;
using LibUnity.Frontend.State.Subjects;
using Debug = UnityEngine.Debug;

namespace LibUnity.Frontend.State
{
    /// <summary>
    /// 클라이언트가 참조할 상태를 포함한다.
    /// 체인의 상태를 Setter를 통해서 받은 후, 로컬의 상태로 필터링해서 사용한다.
    /// </summary>
    public class States
    {
        public static States Instance => Game.Instance.States;

        public AgentState AgentState { get; private set; }

        public GoldBalanceState GoldBalanceState { get; private set; }
        
        public void SetAgentState(AgentState state)
        {
            if (state is null)
            {
                Debug.LogWarning($"[{nameof(States)}.{nameof(SetAgentState)}] {nameof(state)} is null.");
                return;
            }
            
            LocalLayer.Instance.InitializeAgent(state);
            AgentState = LocalLayer.Instance.Modify(state);
        }

        public void SetGoldBalanceState(GoldBalanceState goldBalanceState)
        {
            if (goldBalanceState is null)
            {
                Debug.LogWarning($"[{nameof(States)}.{nameof(SetGoldBalanceState)}] {nameof(goldBalanceState)} is null.");
                return;
            }

            GoldBalanceState = LocalLayer.Instance.Modify(goldBalanceState);
            AgentStateSubject.Gold.OnNext(GoldBalanceState.Gold);
        }
    }
}
