using Bencodex.Types;
using Libplanet;

namespace LibUnity.Backend.State
{
    public interface IState
    {
        Address Address { get; }

        IValue Serialize();
    }
}
