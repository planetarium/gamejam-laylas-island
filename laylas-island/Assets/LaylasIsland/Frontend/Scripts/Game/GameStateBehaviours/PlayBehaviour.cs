using System.Collections;
using LaylasIsland.Frontend.Game.Views;
using LaylasIsland.Frontend.UI;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public class PlayBehaviour : IGameStateBehaviour
    {
        private PlayerCharacter _playerCharacter;
        
        public void Enter()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Enter)}()");
            
            _playerCharacter = GameController.Instance.CreatePlayerCharacter().GetComponent<PlayerCharacter>();
            var board = GameController.Instance.Board;
            _playerCharacter.MoveTo(board.StartPoints[Random.Range(0, board.StartPoints.Count)]);
            UIHolder.PlayGameCanvas.gameObject.SetActive(true);
        }

        public IEnumerator CoUpdate()
        {
            yield break;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");

            GameController.Instance.DestroyPlayerCharacter(_playerCharacter.gameObject);
            UIHolder.PlayGameCanvas.gameObject.SetActive(false);
        }
    }
}
