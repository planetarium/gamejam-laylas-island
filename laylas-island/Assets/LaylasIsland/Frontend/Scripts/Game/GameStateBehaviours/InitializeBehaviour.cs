using System.Collections;
using LaylasIsland.Frontend.UI;
using UniRx;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    using Model = SharedGameModel;

    public class InitializeBehaviour : IGameStateBehaviour
    {
        private readonly GameNetworkManager _networkManager;
        private readonly Board _board;

        public GameNetworkManager.JoinOrCreateRoomOptions options;

        public InitializeBehaviour(GameNetworkManager networkManager, Board board)
        {
            _networkManager = networkManager;
            _board = board;
        }

        public void Enter()
        {
            Debug.Log($"[{nameof(InitializeBehaviour)}] {nameof(Enter)}()");

            UIHolder.LoadingCanvas.gameObject.SetActive(true);
        }

        public IEnumerator CoUpdate()
        {
            var exceptionMessage = string.Empty;
            var done = false;
            _networkManager.JoinOrCreateRoom(options).Subscribe(message =>
            {
                exceptionMessage = message;
                done = true;
            });
            yield return new WaitUntil(() => done);
            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                Fail(exceptionMessage);
                yield break;
            }

            done = false;
            _board.Initialize(() => done = true);
            yield return new WaitUntil(() => done);

            Model.State.Value = GameState.Prepare;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(InitializeBehaviour)}] {nameof(Exit)}()");

            UIHolder.LoadingCanvas.gameObject.SetActive(false);
        }

        private static void Fail(string message)
        {
            UIHolder.MessagePopupCanvas.ShowWithASingleButton(
                "Failed",
                message,
                "OK",
                () => Model.State.Value = GameState.None);
        }
    }
}
