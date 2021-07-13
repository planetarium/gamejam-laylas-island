using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
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

        [SerializeField] private Tile _tilePrefab;

        #endregion

        private const int tileCount = 41;
        private readonly List<Tile> _tiles = new List<Tile>();
        private Action _initializeCallback;

        public void Initialize(Action callback)
        {
            _initializeCallback = callback;
            StartCoroutine(SpawnTiles());
        }

        public void Terminate(Action callback)
        {
            for (var i = 0; i < tileCount; i++)
            {
                Destroy(_tiles[i]);
            }
            
            callback?.Invoke();
        }

        private IEnumerator SpawnTiles()
        {
            for (var i = 0; i < tileCount; i++)
            {
                var tile = Instantiate(_tilePrefab, transform);
                tile.Initialize(i, GetLocalPosition(i));
                _tiles.Add(tile);
                
                yield return null;
            }
            
            _initializeCallback?.Invoke();
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
