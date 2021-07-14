using Unity.Mathematics;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.Modules
{
    public class Tile : MonoBehaviour
    {
        #region View

        [SerializeField] private TileSpritesSO _tileSpritesSo;
        [SerializeField] private SpriteRenderer _background;

        #endregion
        
        private int _index;

        public void Initialize(int index, int2 localPosition)
        {
            _index = index;
            transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            name = $"{nameof(Tile)} {_index:00} ({localPosition.x}, {localPosition.y})";
        }
    }
}
