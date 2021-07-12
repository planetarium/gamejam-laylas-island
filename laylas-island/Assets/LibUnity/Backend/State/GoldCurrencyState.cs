using System;
using System.Runtime.Serialization;
using Bencodex;
using Bencodex.Types;
using Libplanet.Assets;

namespace LibUnity.Backend.State
{
    [Serializable]
    public class GoldCurrencyState : BaseState, ISerializable
    {
        public Currency Currency { get; private set; }

        public GoldCurrencyState(Currency currency) : base(Addresses.GoldCurrency)
        {
            Currency = currency;
        }

        public GoldCurrencyState(Dictionary serialized) : base(serialized)
        {
            Currency = new Currency(serialized["currency"]);
        }
        
        public override IValue Serialize() => ((Dictionary) base.Serialize())
            .SetItem("currency", Currency.Serialize());

        protected GoldCurrencyState(SerializationInfo info, StreamingContext context)
            : this((Dictionary) new Codec().Decode((byte[]) info.GetValue("serialized", typeof(byte[]))))
        {
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("serialized", new Codec().Encode(Serialize()));
        }
    }
}
