using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterSpritesSO _characterSpritesSo;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void UpdateSortingOrder(int tileOrder)
        {
            _spriteRenderer.sortingOrder = tileOrder + 100;
        }
    }
}
