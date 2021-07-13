using System;
using System.Collections;
using System.Collections.Generic;
using Boscohyun;
using Cysharp.Threading.Tasks;
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

        public IObservable<Exception> InitializeAsObservable(string gameName, string password)
        {
            if (_state.Value != State.None &&
                _state.Value != State.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.InitializeAsObservable() state: {_state.Value}"));
            }

            Initialize();
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
                });
        }

        public IObservable<Exception> TerminateAsObservable()
        {
            if (_state.Value == State.None ||
                _state.Value == State.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.TerminateAsObservable() state: {_state.Value}"));
            }

            _board.Terminate(() =>
            {
                _board.gameObject.SetActive(false);
                _state.Value = State.None;
            });

            return _state.Where(value => value == State.None).Select(_ => (Exception) null);
        }

        private void Initialize()
        {
            Debug.Log("Initialize() enter");
            _state.Value = State.Initializing;
            _board.gameObject.SetActive(true);
            _board.Initialize(() =>
            {
                _state.Value = State.Prepare;
                Debug.Log("Initialize() exit");
            });
        }
    }
}
