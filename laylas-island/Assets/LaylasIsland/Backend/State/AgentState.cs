using Bencodex.Types;
using LaylasIsland.Backend.Extensions;
using Libplanet;
using Libplanet.Crypto;
using LaylasIsland.Backend.Model;

namespace LaylasIsland.Backend.State
{
    public class AgentState : BaseState
    {
        public new Address Address;
        public Inventory Inventory;

        public static Address CreateAgentAddress()
        {
            var key = new PrivateKey();
            return key.PublicKey.ToAddress();
        }

        public AgentState(Address address) : base(address)
        {
            this.Address = address;
            Inventory = new Inventory(Address);
        }

        public AgentState(Dictionary serialized) : base(serialized)
        {
            Address = new Address(((Binary)serialized["address"]).ByteArray);
            Inventory = new Inventory((Dictionary)serialized["inventory"]);
        }

        public Address GetInventoryAddress() => Address.Derive("inventory");

        public Address GetPostAddress() => Address.Derive("post");
    }
}
