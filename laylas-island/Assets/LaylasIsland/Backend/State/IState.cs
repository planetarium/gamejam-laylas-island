using Bencodex.Types;
using Libplanet;

namespace LaylasIsland.Backend.State
{
    public interface IState
    {
        Address Address { get; }

        IValue Serialize();
    }
}
