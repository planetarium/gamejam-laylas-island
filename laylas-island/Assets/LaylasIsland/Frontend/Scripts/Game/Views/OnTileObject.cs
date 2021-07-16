using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    using UniRx;

    public class OnTileObject : MonoBehaviour, IOnTileObject
    {
        // View
        [SerializeField] private SpriteRenderer _spriteRenderer;
        // ~View

        // Model
        private readonly ReactiveProperty<Tile> _tile = new ReactiveProperty<Tile>();
        // ~Model

        public readonly ReactiveProperty<Sprite> SpriteRP = new ReactiveProperty<Sprite>();

        public bool HasTile => !(_tile.Value is null);

        public Tile Tile => _tile.Value;

        protected virtual void Awake()
        {
            _tile.Subscribe(OnTile).AddTo(gameObject);
        }

        public void MoveTo(Tile tile)
        {
            if (tile is null)
            {
                return;
            }

            _tile.Value = tile;
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _spriteRenderer.sortingOrder = sortingOrder;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            SpriteRP.Value = _spriteRenderer.sprite;
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
            _spriteRenderer.sortingOrder = tile.SortingOrder + 100;
        }
    }
}
