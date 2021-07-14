using System;
using System.Collections;
using System.Collections.Generic;
using Boscohyun;
using LaylasIsland.Frontend.Extensions;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;
    using Model = SharedGameModel;

    public class GameController : MonoSingleton<GameController>
    {
        #region View

        [SerializeField] private GameNetworkManager _networkManager;
        [SerializeField] private Board _board;
        [SerializeField] private Transform _objectsRoot;

        #endregion

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private string _exceptionMessage;

        private void Awake()
        {
            Model.State.Subscribe(OnStateChanged).AddTo(gameObject);
        }

        private void OnDestroy()
        {
            _disposables.DisposeAllAndClear();
        }

        #region Control

        public IObservable<Exception> EnterAsObservable(GameNetworkManager.JoinOrCreateRoomOptions options)
        {
            if (Model.State.Value != GameState.None &&
                Model.State.Value != GameState.InitializeFailed &&
                Model.State.Value != GameState.Terminated)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.InitializeAsObservable() state: {Model.State.Value}"));
            }

            Model.State.Value = GameState.Initializing;
            StartCoroutine(CoInitialize(options));
            return Model.State.Where(value => value == GameState.InitializeFailed || value == GameState.Prepare)
                .Select(_ =>
                {
                    switch (Model.State.Value)
                    {
                        case GameState.InitializeFailed:
                            return new Exception(_exceptionMessage);
                        case GameState.Prepare:
                            return null;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(Model.State), Model.State.Value, null);
                    }
                })
                .First();
        }

        public IObservable<Exception> LeaveAsObservable()
        {
            if (Model.State.Value == GameState.None ||
                Model.State.Value == GameState.InitializeFailed)
            {
                return Observable.Throw<Exception>(
                    new Exception($"GameController.TerminateAsObservable() state: {Model.State.Value}"));
            }

            Model.State.Value = GameState.Terminating;
            StartCoroutine(CoTerminate());
            return Model.State.Where(value => value == GameState.None || value == GameState.Terminated)
                .Select(_ => (Exception) null)
                .First();
        }

        #endregion

        private static void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                case GameState.Initializing:
                case GameState.InitializeFailed:
                    break;
                case GameState.Prepare:
                    break;
                case GameState.Play:
                    break;
                case GameState.End:
                    break;
                case GameState.Terminating:
                    break;
                case GameState.Terminated:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private IEnumerator CoInitialize(GameNetworkManager.JoinOrCreateRoomOptions options)
        {
            var failed = false;
            var done = false;
            _networkManager.JoinOrCreateRoom(options).Subscribe(tuple =>
            {
                var (succeed, errorMessage) = tuple;
                failed = !succeed;
                _exceptionMessage = errorMessage;
                done = true;
            });
            yield return new WaitUntil(() => done);
            if (failed)
            {
                Model.State.Value = GameState.InitializeFailed;
                yield break;
            }

            done = false;
            _board.Initialize(() => done = true);
            yield return new WaitUntil(() => done);

            Model.State.Value = GameState.Prepare;
        }

        private IEnumerator CoTerminate()
        {
            var done = false;
            _networkManager.LeaveRoom().Subscribe(_ => done = true);
            yield return new WaitUntil(() => done);

            done = false;
            _board.Terminate(() => done = true);
            yield return new WaitUntil(() => done);

            Model.State.Value = GameState.Terminated;
        }
    }
}
