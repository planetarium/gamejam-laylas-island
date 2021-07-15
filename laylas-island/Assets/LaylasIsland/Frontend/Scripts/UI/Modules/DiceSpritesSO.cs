using System.Collections.Generic;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.Modules
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DiceSpritesSO")]
    public class DiceSpritesSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> _numberSprites;

        public IReadOnlyList<Sprite> NumberSprites => _numberSprites;
    }
}
