using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using Views;
    
    public class Board : MonoBehaviour
    {
        /*
         *                                     (12, 6)
         * 28,29,30,31,32,33,34,35,36,37,38,39,40,
         * 25,               26,               27,
         * 22,               23,               24,
         * 19,               20,               21,
         * 16,               17,               18,
         * 13,               14,               15,
         * 00,01,02,03,04,05,06,07,08,09,10,11,12,
         * (0, 0)
         */

        #region View

        [SerializeField] private List<Tile> _tiles;
        [SerializeField] private List<Tile> _startPoints;
        [SerializeField] private int _tilesSortingOrderMin;

        #endregion

        public IReadOnlyList<Tile> Tiles => _tiles;
        public IReadOnlyList<Tile> StartPoints => _startPoints;

        private void Reset()
        {
            _tiles = transform.GetComponentsInChildren<Tile>().ToList();
            InitializeTiles();
        }

        // NOTE: 추후에 다양한 종류의 보드로 초기화할 수 있게 합니다.
        // e.i., Initialize(boardData, callback);
        public void Initialize(Action callback)
        {
            InitializeTiles();
            gameObject.SetActive(true);
            callback?.Invoke();
        }

        public void Terminate(Action callback)
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }

        private void InitializeTiles()
        {
            var maxValue = _tilesSortingOrderMin + _tiles.Count - 1;
            for (var i = _tiles.Count; i > 0; i--)
            {
                _tiles[i - 1].UpdateSortingOrder(maxValue);
            }
        }

        private static int2 GetLocalPosition(int index)
        {
            if (index < 13)
            {
                return new int2(index, 0);
            }

            if (index < 28)
            {
                return new int2((index - 13) % 3 * 6, (index - 13) / 3 + 1);
            }

            return new int2(index - 28, 6);
        }
    }
}
