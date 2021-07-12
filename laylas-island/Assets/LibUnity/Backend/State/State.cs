using System;
using Bencodex.Types;
using Libplanet;

namespace LibUnity.Backend.State
{
    [Serializable]
    public abstract class BaseState : IState
    {
        private Address _address;

        public Address Address => _address;

        protected BaseState(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            _address = address;
        }

        protected BaseState(Dictionary serialized)
            : this(serialized.ContainsKey("address")
                ? serialized["address"].ToAddress()
                : throw new ArgumentException($"{nameof(serialized)} does not contain 'address'"))
        {
        }

        protected BaseState(IValue iValue) : this((Dictionary) iValue)
        {
        }
        
        public virtual IValue Serialize() => Dictionary.Empty
            .SetItem("address", _address.Serialize());
    }
}
