using Bencodex.Types;
using Libplanet;

namespace LibUnity.Backend.State
{
    public class AgentState : BaseState
    {
        public AgentState(Address address) : base(address)
        {
        }

        public AgentState(Dictionary serialized) : base(serialized)
        {
        }
    }
}
