using System.Collections.Generic;
using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemSpritesSO")]
    public class ItemSpritesSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> _sprites;

        public IReadOnlyList<Sprite> Sprites => _sprites;
    }
}
