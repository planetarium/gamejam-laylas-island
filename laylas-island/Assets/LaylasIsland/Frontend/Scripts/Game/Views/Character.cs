using System.Linq;
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
            SetSpriteNonPerson();
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

        private void SetSpritePerson()
        {
            var nonPerson = _characterSpritesSo.Sprites.Where(e =>
            {
                var num = int.Parse(e.name);
                return num >= 200022 && num <= 200026;
            }).ToList();
            SetSprite(nonPerson[Random.Range(0, nonPerson.Count)]);
        }

        private void SetSpriteNonPerson()
        {
            var nonPerson = _characterSpritesSo.Sprites.Where(e =>
            {
                var num = int.Parse(e.name);
                return !(num >= 200022 && num <= 200026);
            }).ToList();
            SetSprite(nonPerson[Random.Range(0, nonPerson.Count)]);
        }
    }
}
