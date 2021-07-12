using System.Collections.Generic;
using System.Linq;
using Bencodex.Types;
using Libplanet;

namespace LaylasIsland.Backend.State
{
    public class InventoryState : BaseState
    {
        public List<Address> _itemAddresses;
        
        public InventoryState(Address address) : base(address)
        {
        }

        public InventoryState(Dictionary serialized) : base(serialized)
        {
        }

        // public override IValue Serialize() => ((Dictionary) base.Serialize())
        //     .SetItem("item-addresses", _itemAddresses.OrderBy(e => e.ToHex()).Serialize());
    }
}
