using UnityEngine;

namespace LaylasIsland.Frontend.Game.Views
{
    public class Item : OnTileObject
    {
        [SerializeField] private ItemSpritesSO _itemSpritesSo;

        protected override void Awake()
        {
            base.Awake();
            SetSprite(_itemSpritesSo.Sprites[Random.Range(0, _itemSpritesSo.Sprites.Count)]);
        }
        
        public void SetSpriteByName(string spriteName)
        {
            foreach (var sprite in _itemSpritesSo.Sprites)
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
