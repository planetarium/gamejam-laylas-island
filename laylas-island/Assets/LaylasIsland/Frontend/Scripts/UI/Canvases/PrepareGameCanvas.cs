using System.Collections.Generic;
using LaylasIsland.Frontend.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using GameModel = SharedGameModel;

    public class PrepareGameCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private List<Image> _bluePlayerViews;
        [SerializeField] private List<Image> _redPlayerViews;

        #endregion

        #region Model

        #endregion

        private void Awake()
        {
            GameModel.BluePlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) GameModel.BluePlayers, _bluePlayerViews))
                .Merge(GameModel.BluePlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) GameModel.BluePlayers, _bluePlayerViews)))
                .Subscribe(SetPlayers)
                .AddTo(gameObject);
            GameModel.RedPlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) GameModel.RedPlayers, _redPlayerViews))
                .Merge(GameModel.RedPlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) GameModel.RedPlayers, _redPlayerViews)))
                .Subscribe(SetPlayers)
                .AddTo(gameObject);
        }

        private void OnEnable()
        {
            UIHolder.HeaderCanvas.Show(
                () =>
                {
                    GameController.Instance.LeaveAsObservable().First().Subscribe(_ =>
                    {
                        gameObject.SetActive(false);
                        UIHolder.MainCanvas.gameObject.SetActive(true);
                    });
                },
                HeaderCanvas.Element.Back,
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);
        }

        private static void SetPlayers((IReadOnlyList<Player> models, List<Image> views) tuple)
        {
            var (models, views) = tuple;
            for (var i = 0; i < views.Count; i++)
            {
                if (i >= models.Count)
                {
                    views[i].enabled = false;
                    continue;
                }

                views[i].enabled = true;
                Debug.Log($"({i}) {models[i].nicknameWithHex.Value}");
            }
        }
    }
}
