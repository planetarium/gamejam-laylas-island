using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Character : OnTileObject
    {
        [SerializeField] private CharacterSpritesSO _characterSpritesSo;

        protected override void Awake()
        {
            base.Awake();
            SetSprite(_characterSpritesSo.Sprites[Random.Range(0, _characterSpritesSo.Sprites.Count)]);
        }

        public void SetSpriteByName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                SetSprite(null);
                return;
            }

            foreach (var sprite in _characterSpritesSo.Sprites)
            {
                if (!sprite.name.Equals(spriteName))
                {
                    continue;
                }

                SetSprite(sprite);
                break;
            }
        }
    }
}
