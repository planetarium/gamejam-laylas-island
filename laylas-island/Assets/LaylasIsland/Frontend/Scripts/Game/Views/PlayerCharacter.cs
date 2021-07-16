using Photon.Pun;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    using UniRx;

    [RequireComponent(typeof(PhotonView))]
    public class PlayerCharacter : MonoBehaviourPunCallbacks, IOnTileObject
    {
        [SerializeField] private Character _character;
        [SerializeField] private PhotonView _photonView;

        // Model
        private readonly ReactiveProperty<Tile> _tile = new ReactiveProperty<Tile>();
        // ~Model

        private float _moveCooldown;

        public bool HasTile => _character.HasTile;
        public Tile Tile => _character.Tile;

        private void Awake()
        {
            _character.SpriteRP
                .Subscribe(sprite => _photonView.RPC(
                    "RPCSpriteNameSync",
                    RpcTarget.All,
                    sprite
                        ? sprite.name
                        : string.Empty))
                .AddTo(gameObject);
            _tile.Subscribe(OnTile).AddTo(gameObject);
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            _moveCooldown -= Time.deltaTime;
            if (_moveCooldown > 0f)
            {
                return;
            }

            _moveCooldown = 0.1f;
            Tile tile = null;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                var localPosition = transform.localPosition + Vector3.up;
                if (!GameController.Instance.Board.TryGetTile(localPosition.x, localPosition.y, out tile))
                {
                    return;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                var localPosition = transform.localPosition + Vector3.down;
                if (!GameController.Instance.Board.TryGetTile(localPosition.x, localPosition.y, out tile))
                {
                    return;
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var localPosition = transform.localPosition + Vector3.left;
                if (!GameController.Instance.Board.TryGetTile(localPosition.x, localPosition.y, out tile))
                {
                    return;
                }
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                var localPosition = transform.localPosition + Vector3.right;
                if (!GameController.Instance.Board.TryGetTile(localPosition.x, localPosition.y, out tile))
                {
                    return;
                }
            }

            MoveTo(tile);
        }

        public void MoveTo(Tile tile)
        {
            if (tile is null)
            {
                return;
            }

            _tile.Value = tile;
        }

        private void OnTile(Tile tile)
        {
            if (tile is null)
            {
                return;
            }

            var localPosition = tile.transform.localPosition;
            localPosition.z = 0f;
            transform.localPosition = localPosition;
            _photonView.RPC("RPCSetSortingOrder", RpcTarget.All, tile.SortingOrder + 100);
        }

        [PunRPC]
        private void RPCSetSortingOrder(int value) => _character.SetSortingOrder(value);

        [PunRPC]
        private void RPCSpriteNameSync(string spriteName) => _character.SetSpriteByName(spriteName);
    }
}
