using Bencodex.Types;
using Libplanet;
using Libplanet.Assets;

namespace LibUnity.Backend.State
{
    public class GoldBalanceState : BaseState
    {
        public readonly FungibleAssetValue Gold;

        public GoldBalanceState(Address address, FungibleAssetValue gold) : base(address) =>
            Gold = gold;

        public GoldBalanceState(Dictionary serialized) : base(serialized) =>
            Gold = serialized[nameof(Gold)].ToFungibleAssetValue();

        public GoldBalanceState Add(FungibleAssetValue adder) =>
            new GoldBalanceState(Address, Gold + adder);

        public object Clone() =>
            MemberwiseClone();

        public Address Address { get; }

        public override IValue Serialize() => ((Dictionary) base.Serialize())
            .SetItem(nameof(Gold), Gold.Serialize());
    }
}
