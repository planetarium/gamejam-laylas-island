using System;
using System.Collections.Generic;
using System.Linq;
using Bencodex.Types;
using LaylasIsland.Backend.Action.Exceptions;
using LaylasIsland.Backend.State;
using Libplanet;

namespace LaylasIsland.Backend.Model
{
    /// <summary>
    /// This is a model class of shop state.
    /// </summary>
    [Serializable]
    public class Inventory : BaseState
    {

        private readonly Dictionary<Guid, Item> _items = new Dictionary<Guid, Item>();

        public IReadOnlyDictionary<Guid, Item> Items => _items;

        public Inventory(Address address) : base(address)
        {
        }

        public Inventory(Dictionary serialized) : base(serialized)
        {
            _items = ((Dictionary) serialized["items"]).ToDictionary(
                kv => kv.Key.ToGuid(),
                kv => new Item((Dictionary) kv.Value));
        }

        public override IValue Serialize() =>
#pragma warning disable LAA1002
            new Dictionary(new Dictionary<IKey, IValue>
            {
                [(Text) "items"] = new Dictionary(
                    _items.Select(kv =>
                        new KeyValuePair<IKey, IValue>(
                            (Binary) kv.Key.Serialize(),
                            kv.Value.Serialize()))),
            }.Union((Dictionary) base.Serialize()));
#pragma warning restore LAA1002

        public void Register(Item item)
        {
            var productId = item.ProductId;
            if (_items.ContainsKey(productId))
            {
                throw new ItemAlreadyContainedException($"Aborted as the item already registered # {productId}.");
            }
            _items[productId] = item;
        }

        #region Unregister

        public void Unregister(Item shopItem)
        {
            Unregister(shopItem.ProductId);
        }

        public void Unregister(Guid productId)
        {
            if (!TryUnregister(productId, out _))
            {
                throw new FailedToUnregisterItemException(
                    $"{nameof(_items)}, {productId}");
            }
        }

        public bool TryUnregister(
            Guid productId,
            out Item unregisteredItem)
        {
            if (!_items.ContainsKey(productId))
            {
                unregisteredItem = null;
                return false;
            }

            var targetShopItem = _items[productId];
            _items.Remove(productId);
            unregisteredItem = targetShopItem;
            return true;
        }

        #endregion

        public bool TryGet(
            Address sellerAgentAddress,
            Guid productId,
            out Item shopItem)
        {
            if (!_items.ContainsKey(productId))
            {
                shopItem = null;
                return false;
            }

            shopItem = _items[productId];
            if (shopItem.SellerAgentAddress.Equals(sellerAgentAddress))
            {
                return true;
            }

            shopItem = null;
            return false;
        }
    }
}
