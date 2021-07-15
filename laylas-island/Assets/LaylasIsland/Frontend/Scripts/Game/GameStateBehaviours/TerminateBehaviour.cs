using System.Collections;
using LaylasIsland.Frontend.UI;
using UniRx;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    using Model = SharedGameModel;

    public class TerminateBehaviour : IGameStateBehaviour
    {
        private readonly GameNetworkManager _networkManager;
        private readonly Board _board;

        public TerminateBehaviour(GameNetworkManager networkManager, Board board)
        {
            _networkManager = networkManager;
            _board = board;
        }

        public void Enter()
        {
            Debug.Log($"[{nameof(TerminateBehaviour)}] {nameof(Enter)}()");
            
            UIHolder.LoadingCanvas.gameObject.SetActive(true);
        }

        public IEnumerator CoUpdate()
        {
            var done = false;
            _networkManager.LeaveRoom().Subscribe(_ => done = true);
            yield return new WaitUntil(() => done);

            done = false;
            _board.Terminate(() => done = true);
            yield return new WaitUntil(() => done);

            Model.State.Value = GameState.None;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(TerminateBehaviour)}] {nameof(Exit)}()");
            
            UIHolder.LoadingCanvas.gameObject.SetActive(false);
            UIHolder.PrepareGameCanvas.gameObject.SetActive(false);
            UIHolder.PlayGameCanvas.gameObject.SetActive(false);
            UIHolder.EndGameCanvas.gameObject.SetActive(false);
            UIHolder.MainCanvas.gameObject.SetActive(true);
        }
    }
}
