using System.Collections;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public class EndBehaviour : IGameStateBehaviour
    {
        public void Enter()
        {
            Debug.Log($"[{nameof(EndBehaviour)}] {nameof(Exit)}()");
        }

        public IEnumerator CoUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(EndBehaviour)}] {nameof(Exit)}()");
        }
    }
}
