using Bencodex.Types;
using Libplanet;
using Libplanet.Assets;

namespace LaylasIsland.Backend.State
{
    public class GoldBalanceState : BaseState
    {
        public readonly FungibleAssetValue Gold;

        public GoldBalanceState(Address address, FungibleAssetValue gold) : base(address) =>
            Gold = gold;

        public GoldBalanceState(Dictionary serialized) : base(serialized) =>
            Gold = serialized["gold"].ToFungibleAssetValue();

        public GoldBalanceState Add(FungibleAssetValue adder) =>
            new GoldBalanceState(Address, Gold + adder);

        public object Clone() =>
            MemberwiseClone();

        public override IValue Serialize() => ((Dictionary) base.Serialize())
            .SetItem("gold", Gold.Serialize());
    }
}
