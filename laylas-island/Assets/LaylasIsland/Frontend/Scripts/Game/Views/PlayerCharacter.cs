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
            var localPosition = tile.transform.localPosition;
            localPosition.z = 0f;
            transform.localPosition = localPosition;

            _character.UpdateSortingOrder(tile.SortingOrder);
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
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // FIXME: get tile and MoveTo(tile);
                transform.localPosition += Vector3.up;
                return;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                // FIXME: get tile and MoveTo(tile);
                transform.localPosition += Vector3.down;
                return;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // FIXME: get tile and MoveTo(tile);
                transform.localPosition += Vector3.left;
                return;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                // FIXME: get tile and MoveTo(tile);
                transform.localPosition += Vector3.right;
            }
        }
    }
}
