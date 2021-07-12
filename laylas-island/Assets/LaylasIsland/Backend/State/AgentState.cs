using Bencodex.Types;
using LaylasIsland.Backend.Extensions;
using Libplanet;

namespace LaylasIsland.Backend.State
{
    public class AgentState : BaseState
    {
        public AgentState(Address address) : base(address)
        {
        }

        public AgentState(Dictionary serialized) : base(serialized)
        {
        }

        public Address GetInventoryAddress() => Address.Derive("inventory");

        public Address GetPostAddress() => Address.Derive("post");
    }
}
