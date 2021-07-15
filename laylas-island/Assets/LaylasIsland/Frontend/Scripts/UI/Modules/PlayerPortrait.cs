using System.Linq;
using LaylasIsland.Frontend.Game;
using LaylasIsland.Frontend.UI.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Modules
{
    public class PlayerPortrait : MonoBehaviour
    {
        // View

        [SerializeField] private PortraitSpritesSO _spritesSo;
        [SerializeField] private Image _portrait;

        // ~View

        public void Show(Player player)
        {
            // player.
            var sprite = _spritesSo.Sprites.FirstOrDefault(e =>
                e.name.Equals(player.portrait.Value));
            if (!(sprite is null))
            {
                _portrait.overrideSprite = sprite;
            }
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
