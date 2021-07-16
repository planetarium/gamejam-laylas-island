using System;
using System.Collections.Generic;
using System.Linq;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.Game;
using LaylasIsland.Frontend.UI.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;

    public class PlayGameCanvas : MonoBehaviour
    {
        // View

        [SerializeField] private TextMeshProUGUI _blueScoreText;
        [SerializeField] private TextMeshProUGUI _redScoreText;
        [SerializeField] private List<PlayerPortraitWithScore> _portraits;
        [SerializeField] private Button _diceButton;
        [SerializeField] private DiceGroup _diceGroup;

        // ~View

        private readonly List<IDisposable> _disposablesOnEnable = new List<IDisposable>();

        private readonly ReactiveCollection<(Player player, int score)> _portraitsModel
            = new ReactiveCollection<(Player player, int score)>();

        private void Awake()
        {
            _portraitsModel.ObserveCountChanged().Select(_ => Unit.Default)
                .Merge(_portraitsModel.ObserveReset())
                .Subscribe(_ => UpdatePlayersAndScores())
                .AddTo(gameObject);

            _diceButton.OnClickAsObservable()
                .Subscribe(_ => _diceGroup.ShowAndPlay(Random.Range(1, 7)))
                .AddTo(gameObject);
        }

        private void OnEnable()
        {
            SharedGameModel.BluePlayers.ObserveCountChanged()
                .Merge(SharedGameModel.RedPlayers.ObserveCountChanged())
                .Subscribe(_ =>
                {
                    foreach (var player in SharedGameModel.BluePlayers)
                    {
                        var key = player.nicknameWithHex.Value;
                        if (_portraitsModel.Any(e => e.player.nicknameWithHex.Value?.Equals(key) ?? false))
                        {
                            continue;
                        }

                        _portraitsModel.Add((
                            player,
                            SharedGameModel.BluePlayerScores.ContainsKey(key)
                                ? SharedGameModel.BluePlayerScores[key]
                                : 0
                        ));
                    }

                    foreach (var player in SharedGameModel.RedPlayers)
                    {
                        var key = player.nicknameWithHex.Value;
                        if (_portraitsModel.Any(e => e.player.nicknameWithHex.Value.Equals(key)))
                        {
                            continue;
                        }

                        _portraitsModel.Add((
                            player,
                            SharedGameModel.RedPlayerScores.ContainsKey(key)
                                ? SharedGameModel.RedPlayerScores[key]
                                : 0
                        ));
                    }

                    foreach (var key in _portraitsModel
                        .Select(tuple => tuple.player.nicknameWithHex.Value)
                        .Where(key => !SharedGameModel.BluePlayers.Any(e => e.nicknameWithHex.Value.Equals(key)))
                        .Where(key => !SharedGameModel.RedPlayers.Any(e => e.nicknameWithHex.Value.Equals(key)))
                        .ToList())
                    {
                        try
                        {
                            var target = _portraitsModel.First(tuple => tuple.player.nicknameWithHex.Value.Equals(key));
                            _portraitsModel.Remove(target);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                })
                .AddTo(_disposablesOnEnable);

            SharedGameModel.BluePlayerScores.ObserveCountChanged().Select(_ => Unit.Default)
                .Merge(SharedGameModel.BluePlayerScores.ObserveReplace().Select(_ => Unit.Default))
                .Merge(SharedGameModel.BluePlayerScores.ObserveReset())
                .Subscribe(_ =>
                {
                    for (var i = 0; i < _portraitsModel.Count; i++)
                    {
                        var (player, _) = _portraitsModel[i];
                        var key = player.nicknameWithHex.Value;
                        if (!SharedGameModel.BluePlayerScores.ContainsKey(key))
                        {
                            Debug.LogWarning(
                                $"{nameof(_portraitsModel)} does not contains key({key})");
                        }

                        _portraitsModel[i] = (player, SharedGameModel.BluePlayerScores[key]);
                    }
                }).AddTo(_disposablesOnEnable);

            SharedGameModel.RedPlayerScores.ObserveCountChanged().Select(_ => Unit.Default)
                .Merge(SharedGameModel.RedPlayerScores.ObserveReplace().Select(_ => Unit.Default))
                .Merge(SharedGameModel.RedPlayerScores.ObserveReset())
                .Subscribe(_ =>
                {
                    for (var i = 0; i < _portraitsModel.Count; i++)
                    {
                        var (player, _) = _portraitsModel[i];
                        var key = player.nicknameWithHex.Value;
                        if (!SharedGameModel.RedPlayerScores.ContainsKey(key))
                        {
                            Debug.LogWarning(
                                $"{nameof(_portraitsModel)} does not contains key({key})");
                        }

                        _portraitsModel[i] = (player, SharedGameModel.RedPlayerScores[key]);
                    }
                }).AddTo(_disposablesOnEnable);

            SharedGameModel.BlueScore.Subscribe(value =>
            {
                _blueScoreText.text = value == 0
                    ? "-"
                    : value.ToString();
            }).AddTo(_disposablesOnEnable);

            SharedGameModel.RedScore.Subscribe(value =>
            {
                _redScoreText.text = value == 0
                    ? "-"
                    : value.ToString();
            }).AddTo(_disposablesOnEnable);

            UIHolder.HeaderCanvas.Show(
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);

            InitPortraitsModel();
            UpdatePlayersAndScores();
        }

        private void OnDisable()
        {
            _disposablesOnEnable.DisposeAllAndClear();

            _diceGroup.Hide();
        }

        private void InitPortraitsModel()
        {
            _portraitsModel.Clear();

            foreach (var player in SharedGameModel.BluePlayers)
            {
                _portraitsModel.Add((player, 0));
            }

            foreach (var player in SharedGameModel.RedPlayers)
            {
                _portraitsModel.Add((player, 0));
            }
        }

        private void UpdatePlayersAndScores()
        {
            for (var i = 0; i < _portraits.Count; i++)
            {
                if (i >= _portraitsModel.Count)
                {
                    _portraits[i].Hide();
                    continue;
                }

                var (player, score) = _portraitsModel[i];
                _portraits[i].Show(player, score);
            }
        }
    }
}
