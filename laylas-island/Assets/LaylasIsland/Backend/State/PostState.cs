using Bencodex.Types;
using Libplanet;

namespace LaylasIsland.Backend.State
{
    public class PostState : BaseState
    {
        public PostState(Address address) : base(address)
        {
        }

        public PostState(Dictionary serialized) : base(serialized)
        {
        }
    }
}
