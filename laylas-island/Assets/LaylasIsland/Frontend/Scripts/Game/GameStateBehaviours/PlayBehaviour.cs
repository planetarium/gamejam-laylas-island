using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<IOnTileObject> _onTileObjects = new List<IOnTileObject>();

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
            var transform = _playerCharacter.transform;
            transform.SetParent(GameController.Instance.ObjectRoot);
            transform.localScale = Vector3.one;

            var board = GameController.Instance.Board;
            _playerCharacter.MoveTo(board.StartPoints[Random.Range(0, board.StartPoints.Count)]);
            _onTileObjects.Add(_playerCharacter);
        }

        private void SpawnMonster()
        {
            if (!TryGetEmptyTile(out var tile))
            {
                return;
            }

            var prefab = Resources.Load("Game/Prefabs/Character");
            var character = ((GameObject) Object.Instantiate(prefab, Vector3.zero, Quaternion.identity))
                .GetComponent<Character>();
            var transform = character.transform;
            transform.SetParent(GameController.Instance.ObjectRoot);
            transform.localScale = Vector3.one;
            
            character.MoveTo(tile);
            _onTileObjects.Add(character);
        }

        private void SpawnItem()
        {
            if (!TryGetEmptyTile(out var tile))
            {
                return;
            }

            var prefab = Resources.Load("Game/Prefabs/Item");
            var item = ((GameObject) Object.Instantiate(prefab, Vector3.zero, Quaternion.identity))
                .GetComponent<Item>();
            var transform = item.transform;
            transform.SetParent(GameController.Instance.ObjectRoot);
            transform.localScale = Vector3.one;
            
            item.MoveTo(tile);
            _onTileObjects.Add(item);
        }

        private bool TryGetEmptyTile(out Tile tile)
        {
            var tiles = GameController.Instance.Board.Tiles.ToList();
            for (var i = _onTileObjects.Count; i > 0; i--)
            {
                var onTileObject = _onTileObjects[i - 1];
                if (!onTileObject.HasTile)
                {
                    continue;
                }

                tiles.Remove(onTileObject.Tile);
            }

            if (tiles.Count == 0)
            {
                Debug.LogWarning("There is no any empty Tile");
                tile = null;
                return false;
            }

            tile = tiles[Random.Range(0, tiles.Count)];
            Debug.LogWarning($"Empty Tile is here. {tile.Index}");
            return true;
        }
    }
}
