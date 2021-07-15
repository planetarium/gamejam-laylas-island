using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.UI;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    using UniRx;
    using Model = SharedGameModel;

    public class PrepareBehaviour : IGameStateBehaviour
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private CancellationTokenSource _countdownAndPlayCts;

        public void Enter()
        {
            Debug.Log($"[{nameof(PrepareBehaviour)}] {nameof(Enter)}()");

            // Model
            Model.BluePlayers.ObserveCountChanged()
                .CombineLatest(Model.RedPlayers.ObserveCountChanged(), (e1, e2) => (e1, e2))
                .Subscribe(OnPlayerCountChanged)
                .AddTo(_disposables);
            // ~Model

            // View
            UIHolder.CreateGameCanvas.gameObject.SetActive(false);
            UIHolder.JoinGameCanvas.gameObject.SetActive(false);
            UIHolder.PrepareGameCanvas.gameObject.SetActive(true);
            // ~View

            OnPlayerCountChanged((Model.BluePlayers.Count, Model.RedPlayers.Count));
        }

        public IEnumerator CoUpdate()
        {
            yield break;
        }

        public void Exit()
        {
            Debug.Log($"[{nameof(PrepareBehaviour)}] {nameof(Exit)}()");

            _disposables.DisposeAllAndClear();

            UIHolder.PrepareGameCanvas.gameObject.SetActive(false);
        }

        private void OnPlayerCountChanged((int blueCount, int redCount) tuple)
        {
            var (blueCount, redCount) = tuple;
            // if (blueCount == 2 &&
            //     blueCount == redCount)
            if (blueCount == 1)
            {
                _countdownAndPlayCts = new CancellationTokenSource();
                CountdownAndPlayAsync(_countdownAndPlayCts);
            }
            else if (!(_countdownAndPlayCts is null))
            {
                _countdownAndPlayCts.Cancel();
                _countdownAndPlayCts.Dispose();
                _countdownAndPlayCts = null;
            }
        }

        private static async void CountdownAndPlayAsync(CancellationTokenSource cts)
        {
            var value = 5;
            while (value >= 0)
            {
                Model.Countdown.Value = value;
                await UniTask.Delay(TimeSpan.FromSeconds(1d), cancellationToken: cts.Token);
                value--;
            }

            Model.State.Value = GameState.Play;
        }
    }
}
