using Unity.Mathematics;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    public class Tile : MonoBehaviour
    {
        #region View

        [SerializeField] private SpriteRenderer _spriteRenderer;

        #endregion
        
        private int _index;

        public void Initialize(int index, int2 localPosition)
        {
            _index = index;
            transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            name = $"{nameof(Tile)} {_index:00} ({localPosition.x}, {localPosition.y})";
            _spriteRenderer.color = _index % 2 == 0
                ? Color.green
                : Color.gray;
        }
    }
}
