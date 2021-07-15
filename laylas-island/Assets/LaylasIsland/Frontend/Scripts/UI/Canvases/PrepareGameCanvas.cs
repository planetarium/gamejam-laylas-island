using System.Collections.Generic;
using LaylasIsland.Frontend.Game;
using LaylasIsland.Frontend.UI.Modules;
using TMPro;
using UniRx;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using GameModel = SharedGameModel;

    public class PrepareGameCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private List<PlayerPortrait> _bluePlayerPortraits;
        [SerializeField] private List<PlayerPortrait> _redPlayerPortraits;
        [SerializeField] private TextMeshProUGUI _countdownText;
        [SerializeField] private TextMeshProUGUI _messageText;

        #endregion

        #region Model

        private readonly ReactiveProperty<int> _countdown = new ReactiveProperty<int>(-1);
        private readonly ReactiveProperty<string> _message = new ReactiveProperty<string>(string.Empty);

        #endregion

        private void Awake()
        {
            GameModel.BluePlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) GameModel.BluePlayers, _bluePlayerViews: _bluePlayerPortraits))
                .Merge(GameModel.BluePlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) GameModel.BluePlayers, _bluePlayerViews: _bluePlayerPortraits)))
                .Subscribe(SetPlayers)
                .AddTo(gameObject);
            
            GameModel.RedPlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) GameModel.RedPlayers, _redPlayerViews: _redPlayerPortraits))
                .Merge(GameModel.RedPlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) GameModel.RedPlayers, _redPlayerViews: _redPlayerPortraits)))
                .Subscribe(SetPlayers)
                .AddTo(gameObject);

            _countdown.Subscribe(value =>
            {
                if (value < 0)
                {
                    _countdownText.gameObject.SetActive(false);
                }
                else if (value > 0)
                {
                    _countdownText.text = value.ToString();
                    _countdownText.gameObject.SetActive(false);
                }
                else
                {
                    _countdownText.text = "GO!";
                    _countdownText.gameObject.SetActive(false);
                }
            }).AddTo(gameObject);

            _message.Subscribe(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    _messageText.gameObject.SetActive(false);
                }
                else
                {
                    _messageText.text = value;
                    _messageText.gameObject.SetActive(true);
                }
            }).AddTo(gameObject);

            SetPlayers((GameModel.BluePlayers, _bluePlayerPortraits));
            SetPlayers((GameModel.RedPlayers, _redPlayerPortraits));
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

        private static void SetPlayers((IReadOnlyList<Player> models, List<PlayerPortrait> views) tuple)
        {
            Debug.Log($"[{nameof(PrepareGameCanvas)}] {nameof(SetPlayers)}() enter. {tuple.models.Count} players");
            var (models, views) = tuple;
            for (var i = 0; i < views.Count; i++)
            {
                if (i >= models.Count)
                {
                    views[i].Hide();
                    continue;
                }

                views[i].Show(models[i]);
                Debug.Log($"({i}) {models[i].nicknameWithHex.Value}");
            }
        }
    }
}
