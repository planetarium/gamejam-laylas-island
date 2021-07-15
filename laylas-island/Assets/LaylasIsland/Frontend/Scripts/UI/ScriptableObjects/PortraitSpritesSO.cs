using System.Collections.Generic;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PortraitSpritesSO")]
    public class PortraitSpritesSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> _sprites;

        public IReadOnlyList<Sprite> Sprites => _sprites;
    }
}
