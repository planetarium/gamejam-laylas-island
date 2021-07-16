using System;
using System.Collections.Generic;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.Game;
using LaylasIsland.Frontend.UI.Modules;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;

    public class PrepareGameCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private List<PlayerPortrait> _bluePlayerPortraits;
        [SerializeField] private List<PlayerPortrait> _redPlayerPortraits;
        [SerializeField] private TextMeshProUGUI _countdownText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _startButton;

        #endregion

        #region Model

        private readonly ReactiveProperty<string> _message = new ReactiveProperty<string>(string.Empty);

        #endregion

        private readonly List<IDisposable> _disposablesOnEnable = new List<IDisposable>();

        private void Awake()
        {
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

            
            if (PhotonNetwork.IsMasterClient)
            {
                _startButton.interactable = false;
                _startButton.OnClickAsObservable()
                    .Subscribe(_ => GameController.Instance.NetworkManager.OnClickStartButton())
                    .AddTo(gameObject);
            }
            else
            {
                _startButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            SharedGameModel.BluePlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) SharedGameModel.BluePlayers,
                    _bluePlayerViews: _bluePlayerPortraits))
                .Merge(SharedGameModel.BluePlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) SharedGameModel.BluePlayers,
                        _bluePlayerViews: _bluePlayerPortraits))).Subscribe(SetPlayers)
                .AddTo(_disposablesOnEnable);

            SharedGameModel.RedPlayers.ObserveCountChanged()
                .Select(_ => ((IReadOnlyList<Player>) SharedGameModel.RedPlayers, _redPlayerViews: _redPlayerPortraits))
                .Merge(SharedGameModel.RedPlayers.ObserveMove()
                    .Select(_ => ((IReadOnlyList<Player>) SharedGameModel.RedPlayers,
                        _redPlayerViews: _redPlayerPortraits)))
                .Subscribe(SetPlayers)
                .AddTo(_disposablesOnEnable);

            SharedGameModel.BluePlayers.ObserveCountChanged()
                .CombineLatest(SharedGameModel.RedPlayers.ObserveCountChanged(), (e1, e2) => (e1, e2))
                .Subscribe(OnPlayerCountChanged)
                .AddTo(_disposablesOnEnable);

            SharedGameModel.Countdown.Subscribe(value =>
            {
                if (value < 0)
                {
                    _countdownText.gameObject.SetActive(false);
                }
                else if (value > 0)
                {
                    _countdownText.text = value.ToString();
                    _countdownText.gameObject.SetActive(true);
                }
                else
                {
                    _countdownText.text = "GO!";
                    _countdownText.gameObject.SetActive(true);
                }
            }).AddTo(_disposablesOnEnable);

            SetPlayers((SharedGameModel.BluePlayers, _bluePlayerPortraits));
            SetPlayers((SharedGameModel.RedPlayers, _redPlayerPortraits));

            UIHolder.HeaderCanvas.Show(
                GameController.Leave,
                HeaderCanvas.Element.Back,
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);
        }

        private void OnDisable()
        {
            _disposablesOnEnable.DisposeAllAndClear();
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

        private void OnPlayerCountChanged((int blueCount, int redCount) tuple)
        {
            var (blueCount, redCount) = tuple;
            _startButton.interactable = blueCount == redCount;
        }
    }
}
