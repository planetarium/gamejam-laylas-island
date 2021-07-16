using Photon.Pun;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerCharacter : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Character _character;
        [SerializeField] private PhotonView _photonView;

        private float _moveCooldown;

        public void MoveTo(Tile tile)
        {
            if (tile is null)
            {
                return;
            }
            
            var localPosition = tile.transform.localPosition;
            localPosition.z = 0f;
            transform.localPosition = localPosition;

            _photonView.RPC("RPCUpdateSortingOrder", RpcTarget.All, tile.SortingOrder);
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
        
        [PunRPC]
        private void RPCUpdateSortingOrder(int tileOrder)
        {
            _character.SpriteRenderer.sortingOrder = tileOrder + 100;
        }
    }
}
