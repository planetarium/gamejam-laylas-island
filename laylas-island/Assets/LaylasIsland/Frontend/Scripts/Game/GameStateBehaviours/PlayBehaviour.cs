using System.Collections;
using LaylasIsland.Frontend.UI;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public class PlayBehaviour : IGameStateBehaviour
    {
        public void Enter()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");

            UIHolder.PlayGameCanvas.gameObject.SetActive(true);
        }

        public IEnumerator CoUpdate()
        {
            yield break;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");
            
            UIHolder.PlayGameCanvas.gameObject.SetActive(false);
        }
    }
}
