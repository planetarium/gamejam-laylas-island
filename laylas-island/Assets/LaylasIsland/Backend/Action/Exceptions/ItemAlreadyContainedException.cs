using System;
using System.Runtime.Serialization;

namespace LaylasIsland.Backend.Action.Exceptions
{
    [Serializable]
    public class ItemAlreadyContainedException : Exception
    {
        public ItemAlreadyContainedException(string message) : base(message)
        {
        }

        protected ItemAlreadyContainedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
