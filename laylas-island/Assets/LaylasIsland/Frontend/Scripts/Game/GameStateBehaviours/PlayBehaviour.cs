using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using LaylasIsland.Frontend.Game.Views;
using LaylasIsland.Frontend.UI;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public class PlayBehaviour : IGameStateBehaviour
    {
        private PlayerCharacter _playerCharacter;
        private int[] _scenario;
        private int _currentPlotIndex;

        public void Enter()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Enter)}()");
            UIHolder.PlayGameCanvas.gameObject.SetActive(true);

            PlayScenario();
        }

        public IEnumerator CoUpdate()
        {
            yield break;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(PlayBehaviour)}] {nameof(Exit)}()");

            PhotonNetwork.Destroy(_playerCharacter.gameObject);
            UIHolder.PlayGameCanvas.gameObject.SetActive(false);
        }

        private void PlayScenario()
        {
            _scenario = new[] {1, 2, 3, 2, 2, 2, 3};
            _currentPlotIndex = -1;
            NextPlot();
        }

        private void NextPlot()
        {
            _currentPlotIndex++;
            if (_currentPlotIndex >= _scenario.Length - 1)
            {
                _currentPlotIndex = 1;
            }

            var plot = _scenario[_currentPlotIndex];
            switch (plot)
            {
                case 1:
                    SpawnPlayerCharacter();
                    break;
                case 2:
                    SpawnMonster();
                    break;
                case 3:
                    SpawnItem();
                    break;
            }

            var cts = new CancellationTokenSource();
            UniTask.Delay(TimeSpan.FromSeconds(Random.Range(1f, 3f)), cancellationToken: cts.Token)
                .GetAwaiter().OnCompleted(NextPlot);
        }

        private void SpawnPlayerCharacter()
        {
            _playerCharacter = PhotonNetwork
                .Instantiate("Game/Prefabs/PlayerCharacter", Vector3.zero, Quaternion.identity)
                .GetComponent<PlayerCharacter>();
            _playerCharacter.transform.SetParent(GameController.Instance.ObjectRoot);

            var board = GameController.Instance.Board;
            _playerCharacter.MoveTo(board.StartPoints[Random.Range(0, board.StartPoints.Count)]);
        }

        private static void SpawnMonster()
        {
            var prefab = Resources.Load("Game/Prefabs/Character");
            var character = ((GameObject) Object.Instantiate(prefab, Vector3.zero, Quaternion.identity))
                .GetComponent<Character>();
            character.transform.SetParent(GameController.Instance.ObjectRoot);

            var tiles = GameController.Instance.Board.Tiles;
            character.MoveTo(tiles[Random.Range(0, tiles.Count)]);
        }

        private static void SpawnItem()
        {
            var prefab = Resources.Load("Game/Prefabs/Item");
            var item = ((GameObject) Object.Instantiate(prefab, Vector3.zero, Quaternion.identity))
                .GetComponent<Item>();
            item.transform.SetParent(GameController.Instance.ObjectRoot);

            var tiles = GameController.Instance.Board.Tiles;
            item.MoveTo(tiles[Random.Range(0, tiles.Count)]);
        }
    }
}
