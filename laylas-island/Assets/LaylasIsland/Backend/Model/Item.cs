using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Bencodex;
using Bencodex.Types;
using LaylasIsland.Backend.State;
using Libplanet;
using Libplanet.Assets;

namespace LaylasIsland.Backend.Model
{
    [Serializable]
    public class Item
    {
        protected static readonly Codec Codec = new Codec();
        
        public readonly Address SellerAgentAddress;
        public readonly Guid ProductId;
        public readonly FungibleAssetValue Price;

        public Item(
            Address sellerAgentAddress,
            Guid productId,
            FungibleAssetValue price
        )
        {
            SellerAgentAddress = sellerAgentAddress;
            ProductId = productId;
            Price = price;
        }

        public Item(Dictionary serialized)
        {
            SellerAgentAddress = serialized["sellerAgentAddress"].ToAddress();
            ProductId = serialized["productId"].ToGuid();
            Price = serialized["price"].ToFungibleAssetValue();
        }
        
        protected Item(SerializationInfo info, StreamingContext _)
            : this((Dictionary) Codec.Decode((byte[]) info.GetValue("serialized", typeof(byte[]))))
        {
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            
            info.AddValue("serialized", Codec.Encode(Serialize()));
        }

        public IValue Serialize()
        {
            var innerDictionary = new Dictionary<IKey, IValue>
            {
                [(Text) "sellerAgentAddress"] = SellerAgentAddress.Serialize(),
                [(Text) "productId"] = ProductId.Serialize(),
                [(Text) "price"] = Price.Serialize(),
            };

            return new Dictionary(innerDictionary);
        }


        public IValue SerializeBackup1() =>
            new Dictionary(new Dictionary<IKey, IValue>
            {
                [(Text) "sellerAgentAddress"] = SellerAgentAddress.Serialize(),
                [(Text) "productId"] = ProductId.Serialize(),
                [(Text) "price"] = Price.Serialize(),
            });

        protected bool Equals(Item other)
        {
            return ProductId.Equals(other.ProductId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode();
        }
    }
}
