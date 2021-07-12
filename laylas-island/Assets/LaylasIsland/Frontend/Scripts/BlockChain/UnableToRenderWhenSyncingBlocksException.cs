using System;
using System.Runtime.Serialization;

namespace LaylasIsland.Frontend.BlockChain
{
    [Serializable]
    public class UnableToRenderWhenSyncingBlocksException : Exception
    {
        public UnableToRenderWhenSyncingBlocksException() : base()
        {
        }

        public UnableToRenderWhenSyncingBlocksException(string message) : base(message)
        {
        }

        public UnableToRenderWhenSyncingBlocksException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
