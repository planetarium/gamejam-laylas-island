using System;
using System.Collections;
using System.Collections.Generic;
using Boscohyun;
using LaylasIsland.Frontend.Extensions;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;

    public class GameController : MonoSingleton<GameController>
    {
        private enum State
        {
            None = 0,
            Initializing,
            InitializeFailed,
            Prepare,
            Play,
            End,
            Terminating,
        }

        #region View

        [SerializeField] private GameNetworkManager _networkManager;
        [SerializeField] private Board _board;

        #endregion

        #region Model

        private readonly ReactiveProperty<State> _state = new ReactiveProperty<State>(default);

        #endregion

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void OnDestroy()
        {
            _disposables.DisposeAllAndClear();
        }

        public IObservable<Exception> InitializeAsObservable(GameNetworkManager.JoinOrCreateRoomOptions options)
        {
            if (_state.Value != State.None &&
                _state.Value != State.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.InitializeAsObservable() state: {_state.Value}"));
            }

            _state.Value = State.Initializing;
            StartCoroutine(CoInitialize(options));
            return _state.Where(value => value == State.InitializeFailed || value == State.Prepare)
                .Select(_ =>
                {
                    switch (_state.Value)
                    {
                        case State.InitializeFailed:
                            return new Exception("GameController.InitializeAsObservable() initialize failed");
                        case State.Prepare:
                            return null;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_state), _state.Value, null);
                    }
                })
                .First();
        }

        public IObservable<Exception> TerminateAsObservable()
        {
            if (_state.Value == State.None ||
                _state.Value == State.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.TerminateAsObservable() state: {_state.Value}"));
            }

            _board.Terminate(() => _state.Value = State.None);

            return _state.Where(value => value == State.None).Select(_ => (Exception) null);
        }

        private IEnumerator CoInitialize(GameNetworkManager.JoinOrCreateRoomOptions options)
        {
            var done = false;
            _networkManager.JoinOrCreateRoom(options).Subscribe(_ => done = true);
            yield return new WaitUntil(() => done);

            done = false;
            _board.Initialize(() => done = true);
            yield return new WaitUntil(() => done);

            _state.Value = State.Prepare;
        }
    }
}
