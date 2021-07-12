using Bencodex.Types;
using Libplanet;

namespace LaylasIsland.Backend.State
{
    public class ItemState : BaseState
    {
        public ItemState(Address address) : base(address)
        {
        }

        public ItemState(Dictionary serialized) : base(serialized)
        {
        }
    }
}
