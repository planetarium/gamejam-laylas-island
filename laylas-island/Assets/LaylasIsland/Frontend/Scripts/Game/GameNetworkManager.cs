using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    public class GameNetworkManager : MonoBehaviourPunCallbacks
    {
        public enum State
        {
            ConnectingToMaster = 0,
            ConnectedToMaster,
            EnteringRoom,
            EnterRoomFailed,
            EnteredRoom,
        }

        public enum JoinOrCreate
        {
            Join,
            Create,
        }

        public readonly struct JoinOrCreateRoomOptions
        {
            public readonly JoinOrCreate joinOrCreate;
            public readonly string roomName;
            public readonly string password;

            public JoinOrCreateRoomOptions(JoinOrCreate joinOrCreate, string roomName, string password)
            {
                this.joinOrCreate = joinOrCreate;
                this.roomName = roomName;
                this.password = password;
            }
        }

        #region Model

        private readonly ReactiveProperty<State> _state = new ReactiveProperty<State>();

        #endregion

        private void Awake()
        {
            _state.Value = State.ConnectingToMaster;
            PhotonNetwork.ConnectUsingSettings();
        }

        #region Control

        public IObservable<bool> JoinOrCreateRoom(JoinOrCreateRoomOptions options)
        {
            if (_state.Value == State.ConnectingToMaster ||
                _state.Value == State.EnteringRoom)
            {
                Debug.LogError($"Create Room Failed: {_state.Value.ToString()}");
                return Observable.Empty(false);
            }

            _state.Value = State.EnteringRoom;
            return _state.Where(e => e == State.EnterRoomFailed || e == State.EnteredRoom)
                .Select(e => e == State.EnteredRoom)
                .First()
                .DoOnSubscribe(() =>
                {
                    switch (options.joinOrCreate)
                    {
                        case JoinOrCreate.Join:
                            JoinRoom(options.roomName);
                            break;
                        case JoinOrCreate.Create:
                            CreateRoom(options.roomName);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        private void CreateRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError($"{nameof(roomName)} is null or empty");
                _state.Value = State.EnterRoomFailed;
                return;
            }

            PhotonNetwork.CreateRoom(roomName);
        }

        private static void JoinRoom(string roomName = default)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }

        #endregion

        #region Override

        public override void OnConnectedToMaster()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnConnectedToMaster)}() enter");
            _state.Value = State.ConnectedToMaster;
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnCreatedRoom)}() enter");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnCreateRoomFailed)}() enter. {returnCode} {message}");
            _state.Value = State.EnterRoomFailed;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnJoinedRoom)}() enter");
            _state.Value = State.EnteredRoom;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning(
                $"[{nameof(GameNetworkManager)}] {nameof(OnJoinRoomFailed)}() enter. {returnCode}, {message}");
            _state.Value = State.EnterRoomFailed;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarning(
                $"[{nameof(GameNetworkManager)}] {nameof(OnJoinRandomFailed)}() enter. {returnCode}, {message}");
            _state.Value = State.EnterRoomFailed;
        }

        #endregion
    }
}
