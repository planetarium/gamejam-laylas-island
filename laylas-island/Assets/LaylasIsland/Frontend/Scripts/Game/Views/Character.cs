using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterSpritesSO _characterSpritesSo;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer.sprite = _characterSpritesSo.Sprites[Random.Range(0, _characterSpritesSo.Sprites.Count)];
        }

        public void MoveTo(Tile tile)
        {
            var localPosition = tile.transform.localPosition;
            localPosition.z = 0f;
            transform.localPosition = localPosition;

            UpdateSortingOrder(tile.SortingOrder);
        }

        [PunRPC]
        private void UpdateSortingOrder(int tileOrder)
        {
            _spriteRenderer.sortingOrder = tileOrder + 100;
        }
    }
}
