using System;
using System.Collections;
using System.Collections.Generic;
using LaylasIsland.Frontend.Extensions;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;

    public class GameController : IDisposable
    {
        public enum State
        {
            None = 0,
            Initializing,
            InitializeFailed,
            Prepare,
            Play,
            End,
            Terminating,
        }

        #region Model

        private readonly ReactiveProperty<State> _state = new ReactiveProperty<State>(default);

        #endregion

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void Dispose()
        {
            _disposables.DisposeAllAndClear();
        }

        public IObservable<Exception> InitializeAsObservable(string name, string password)
        {
            if (_state.Value != State.None &&
                _state.Value != State.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.InitializeAsObservable() state: {_state.Value}"));
            }

            MainThreadDispatcher.StartCoroutine(CoInitialize());
            return _state.Where(value => value != State.None && value != State.Initializing).Select(value =>
            {
                Debug.Log("Select() enter");
                switch (value)
                {
                    case State.InitializeFailed:
                        return new Exception("GameController.InitializeAsObservable() initialize failed");
                    case State.Prepare:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            });
        }

        private IEnumerator CoInitialize()
        {
            Debug.Log("CoInitialize() enter");
            _state.Value = State.Initializing;
            yield return new WaitForSeconds(3f);
            _state.Value = State.InitializeFailed;
            Debug.Log("CoInitialize() exit");
        }
    }
}
