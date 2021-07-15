using System;
using System.Collections.Generic;
using Boscohyun;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.Game.GameStateBehaviours;
using LaylasIsland.Frontend.Game.Views;
using Photon.Pun;
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

        private readonly Dictionary<GameState, IGameStateBehaviour> _stateBehaviours =
            new Dictionary<GameState, IGameStateBehaviour>();

        private GameState _currentBehaviourState;
        private Coroutine _currentBehaviourCoroutine;

        public Board Board => _board;
        public Transform ObjectRoot => _objectsRoot;

        private void Awake()
        {
            _stateBehaviours.Add(GameState.Initializing, new InitializeBehaviour(_networkManager, _board));
            _stateBehaviours.Add(GameState.Prepare, new PrepareBehaviour());
            _stateBehaviours.Add(GameState.Play, new PlayBehaviour());
            _stateBehaviours.Add(GameState.End, new EndBehaviour());
            _stateBehaviours.Add(GameState.Terminating, new TerminateBehaviour(_networkManager, _board));

            Model.State.Subscribe(OnStateChanged).AddTo(gameObject);
        }

        private void OnDestroy()
        {
            _disposables.DisposeAllAndClear();
        }

        #region Control

        public void Enter(GameNetworkManager.JoinOrCreateRoomOptions options)
        {
            if (Model.State.Value != GameState.None)
            {
                Debug.LogError($"GameController.InitializeAsObservable() state: {Model.State.Value}");
            }

            if (TryGetBehaviour<InitializeBehaviour>(GameState.Initializing, out var initializeBehaviour))
            {
                initializeBehaviour.options = options;
            }

            Model.State.Value = GameState.Initializing;
        }

        public static void Leave()
        {
            if (Model.State.Value == GameState.None)
            {
                Debug.LogError($"GameController.TerminateAsObservable() state: {Model.State.Value}");
            }

            Model.State.Value = GameState.Terminating;
        }

        public GameObject CreatePlayerCharacter()
        {
            var go =
                PhotonNetwork.Instantiate("Game/Prefabs/PlayerCharacter", Vector3.zero, Quaternion.identity);
            go.transform.SetParent(ObjectRoot);
            return go;
        }

        public void DestroyPlayerCharacter(GameObject go)
        {
            PhotonNetwork.Destroy(go);
        }

        #endregion

        private bool TryGetBehaviour<T>(GameState state, out T behaviour) where T : IGameStateBehaviour
        {
            try
            {
                behaviour = (T) _stateBehaviours[state];
                return true;
            }
            catch
            {
                behaviour = default;
                return false;
            }
        }

        private void OnStateChanged(GameState state)
        {
            if (_stateBehaviours.ContainsKey(_currentBehaviourState))
            {
                if (!(_currentBehaviourCoroutine is null))
                {
                    StopCoroutine(_currentBehaviourCoroutine);
                }

                _stateBehaviours[_currentBehaviourState].Exit();
            }

            _currentBehaviourState = state;
            if (!_stateBehaviours.ContainsKey(_currentBehaviourState))
            {
                return;
            }

            var behaviour = _stateBehaviours[_currentBehaviourState];
            behaviour.Enter();
            _currentBehaviourCoroutine = StartCoroutine(behaviour.CoUpdate());
        }
    }
}
