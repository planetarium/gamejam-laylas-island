using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Tile : MonoBehaviour
    {
        #region View

        [SerializeField] private TileSpritesSO _tileSpritesSo;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private int _index;

        #endregion

        public int SortingOrder => _spriteRenderer.sortingOrder;

        public int Index => _index;

        private void Awake()
        {
            name = $"{nameof(Tile)} {_index:00}";
            _spriteRenderer.sprite = _tileSpritesSo.Sprites[Random.Range(0, _tileSpritesSo.Sprites.Count)];
        }

        public void UpdateSortingOrder(int maxOrder)
        {
            _spriteRenderer.sortingOrder = maxOrder - _index;
        }
    }
}
