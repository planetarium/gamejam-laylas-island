using System;
using System.Runtime.Serialization;
using Libplanet;
using Libplanet.Blocks;

namespace LaylasIsland.Backend
{
    [Serializable]
    public class InvalidMinerException : InvalidBlockException
    {
        public Address Miner { get; }
        public InvalidMinerException(string message, Address miner)
            : base(message)
        {
            Miner = miner;
        }

        public InvalidMinerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Miner = (Address) info.GetValue(nameof(Miner), typeof(Address));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Message), Message);
            info.AddValue(nameof(Miner), Miner);
        }
    }
}
