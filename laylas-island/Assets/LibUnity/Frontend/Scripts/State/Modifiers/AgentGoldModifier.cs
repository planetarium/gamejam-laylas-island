using System;
using System.Linq;
using Bencodex;
using Bencodex.Types;
using Libplanet;
using Libplanet.Assets;
using LibUnity.Backend.State;
using UnityEngine;

namespace LibUnity.Frontend.State.Modifiers
{
    [Serializable]
    public class AgentGoldModifier : IAccumulatableStateModifier<GoldBalanceState>
    {
        [SerializeField]
        private string hex;

        [NonSerialized]
        private FungibleAssetValue? _goldCache;

        public bool dirty { get; set; }

        public bool IsEmpty => Gold.Sign == 0;

        private FungibleAssetValue Gold
        {
            get
            {
                if (_goldCache.HasValue)
                {
                    return _goldCache.Value;
                }

                var serialized = (List) new Codec().Decode(ByteUtil.ParseHex(hex));
                _goldCache = FungibleAssetValue.FromRawValue(
                    new Currency(serialized.ElementAt(0)),
                    serialized.ElementAt(1).ToBigInteger());

                return _goldCache.Value;
            }
            set
            {
                var serialized = new List(new[]
                {
                    value.Currency.Serialize(),
                    (Integer) value.RawValue,
                });

                hex = ByteUtil.Hex(new Codec().Encode(serialized));
                _goldCache = null;
            }
        }

        public AgentGoldModifier(FungibleAssetValue gold)
        {
            Gold = gold;
        }

        public AgentGoldModifier(Currency currency, int gold) : this(
            new FungibleAssetValue(currency, gold, 0))
        {
        }

        public void Add(IAccumulatableStateModifier<GoldBalanceState> modifier)
        {
            if (!(modifier is AgentGoldModifier m))
            {
                return;
            }

            Gold += m.Gold;
        }

        public void Remove(IAccumulatableStateModifier<GoldBalanceState> modifier)
        {
            if (!(modifier is AgentGoldModifier m))
            {
                return;
            }

            Gold -= m.Gold;
        }

        public GoldBalanceState Modify(GoldBalanceState state) =>
            state?.Add(Gold);

        public override string ToString()
        {
            return $"{nameof(Gold)}: {Gold}";
        }
    }
}
