using System.Collections;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public class PlayBehaviour : IGameStateBehaviour
    {
        public void Enter()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");
        }

        public IEnumerator CoUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");
        }
    }
}
