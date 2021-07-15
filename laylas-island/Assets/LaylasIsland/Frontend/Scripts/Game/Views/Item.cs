using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemSpritesSO _itemSpritesSo;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private void Awake()
        {
            _spriteRenderer.sprite = _itemSpritesSo.Sprites[Random.Range(0, _itemSpritesSo.Sprites.Count)];
        }

        public void MoveTo(Tile tile)
        {
            var localPosition = tile.transform.localPosition;
            localPosition.z = 0f;
            transform.localPosition = localPosition;

            UpdateSortingOrder(tile.SortingOrder);
        }

        public void UpdateSortingOrder(int tileOrder)
        {
            _spriteRenderer.sortingOrder = tileOrder + 100;
        }
    }
}
