using System.Collections.Generic;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.Modules
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/IconSpritesSO")]
    public class IconSpritesSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> _sprites;

        public IReadOnlyList<Sprite> Sprites => _sprites;
    }
}
