using System;
using System.Runtime.Serialization;

namespace LaylasIsland.Backend.Action.Exceptions
{
    [Serializable]
    public class FailedToUnregisterItemException : Exception
    {
        public FailedToUnregisterItemException(string message) : base(message)
        {
        }

        protected FailedToUnregisterItemException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
