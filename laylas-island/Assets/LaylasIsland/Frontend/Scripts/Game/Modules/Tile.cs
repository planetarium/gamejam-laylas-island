﻿using UnityEngine;

namespace LaylasIsland.Frontend.Game.Modules
{
    public class Tile : MonoBehaviour
    {
        #region View

        [SerializeField] private TileSpritesSO _tileSpritesSo;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private int _index;

        #endregion

        public int SortingOrder => _spriteRenderer.sortingOrder;

        private void Awake()
        {
            name = $"{nameof(Tile)} {_index:00}";
        }

        public void SetSortingOrderMin(int value)
        {
            _spriteRenderer.sortingOrder = value + _index;
        }
    }
}