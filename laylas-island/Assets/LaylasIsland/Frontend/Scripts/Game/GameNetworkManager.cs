using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;
    using Model = SharedGameModel;

    public class GameNetworkManager : MonoBehaviourPunCallbacks
    {
        public enum State
        {
            ConnectingToMaster = 0,
            ConnectedToMaster,
            JoinOrCreateRoom,
            JoinOrCreateRoomFailed,
            JoinedRoom,
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

        private string _stateMessage;

        #endregion

        #region View

        [SerializeField] private PhotonView _photonView;

        #endregion

        private void Awake()
        {
            Model.Player.Subscribe(player =>
            {
                if (player is null)
                {
                    PhotonNetwork.LocalPlayer.NickName = string.Empty;
                    return;
                }

                player.nicknameWithHex
                    .Subscribe(value => PhotonNetwork.LocalPlayer.NickName = value)
                    .AddTo(gameObject);
            }).AddTo(gameObject);

            _stateMessage = string.Empty;
            _state.Value = State.ConnectingToMaster;
            PhotonNetwork.ConnectUsingSettings();
        }

        #region Control

        public IObservable<string> JoinOrCreateRoom(JoinOrCreateRoomOptions options)
        {
            if (_state.Value == State.ConnectingToMaster ||
                _state.Value == State.JoinOrCreateRoom)
            {
                var message = $"Join or Create Room Failed: {_state.Value.ToString()}";
                Debug.LogError(message);
                return Observable.Empty(message);
            }

            _stateMessage = string.Empty;
            _state.Value = State.JoinOrCreateRoom;
            return _state.Where(e => e == State.JoinOrCreateRoomFailed || e == State.JoinedRoom)
                .Select(e => _stateMessage)
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

        public IObservable<Unit> LeaveRoom()
        {
            if (_state.Value != State.JoinedRoom)
            {
                Debug.LogError($"Leave Room Failed: {_state.Value.ToString()}");
                return Observable.Empty(Unit.Default);
            }

            return _state.Where(e => e == State.ConnectedToMaster)
                .Select(e => Unit.Default)
                .First()
                .DoOnSubscribe(() => PhotonNetwork.LeaveRoom());
        }

        private void CreateRoom(string roomName)
        {
            // NOTE: Allow empty room name
            // if (string.IsNullOrEmpty(roomName))
            // {
            //     var message = $"{nameof(roomName)} is null or empty";
            //     Debug.LogError(message);
            //     _stateMessage = message;
            //     _state.Value = State.JoinOrCreateRoomFailed;
            //     return;
            // }

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
            _stateMessage = string.Empty;
            _state.Value = State.ConnectedToMaster;
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnCreatedRoom)}() enter");
            _stateMessage = string.Empty;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnCreateRoomFailed)}() enter. {returnCode} {message}");
            _stateMessage = message;
            _state.Value = State.JoinOrCreateRoomFailed;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnJoinedRoom)}() enter");

            var player = Model.Player.Value;
            _photonView.RPC(
                "AddPlayer",
                RpcTarget.All,
                player.nicknameWithHex.Value,
                player.portrait.Value);

            _stateMessage = string.Empty;
            _state.Value = State.JoinedRoom;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning(
                $"[{nameof(GameNetworkManager)}] {nameof(OnJoinRoomFailed)}() enter. {returnCode}, {message}");
            _stateMessage = message;
            _state.Value = State.JoinOrCreateRoomFailed;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarning(
                $"[{nameof(GameNetworkManager)}] {nameof(OnJoinRandomFailed)}() enter. {returnCode}, {message}");
            _stateMessage = message;
            _state.Value = State.JoinOrCreateRoomFailed;
        }

        public override void OnLeftRoom()
        {
            Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(OnLeftRoom)}() enter");
            _stateMessage = string.Empty;
            _state.Value = State.ConnectedToMaster;
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log(
                $"[{nameof(GameNetworkManager)}] {nameof(OnPlayerLeftRoom)}() enter. {otherPlayer.NickName} {otherPlayer.IsInactive}");

            RemovePlayer(otherPlayer.NickName);
        }

        #endregion

        #region RPC

        [PunRPC]
        private void AddPlayer(string nicknameWithHex, string portrait)
        {
            if (Model.BluePlayers.Count <= Model.RedPlayers.Count)
            {
                Model.BluePlayers.Add(new Player(nicknameWithHex, portrait));
                Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(AddPlayer)}() {nicknameWithHex} added");
            }
            else
            {
                Model.RedPlayers.Add(new Player(nicknameWithHex, portrait));
                Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(AddPlayer)}() {nicknameWithHex} added");
            }
        }

        #endregion

        private static void RemovePlayer(string nicknameWithHex)
        {
            var player = Model.BluePlayers.FirstOrDefault(e =>
                e?.nicknameWithHex.Value.Equals(nicknameWithHex) ?? false);
            if (!(player is null))
            {
                Model.BluePlayers.Remove(player);
                Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(RemovePlayer)}() {nicknameWithHex} removed");
                return;
            }

            player = Model.RedPlayers.FirstOrDefault(e =>
                e?.nicknameWithHex.Value.Equals(nicknameWithHex) ?? false);
            if (!(player is null))
            {
                Model.RedPlayers.Remove(player);
                Debug.Log($"[{nameof(GameNetworkManager)}] {nameof(RemovePlayer)}() {nicknameWithHex} removed");
            }
        }
    }
}
