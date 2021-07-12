using System;
using System.Runtime.Serialization;

namespace LibUnity.RPC.Exceptions
{
    public enum RPCException
    {
        NetworkException = 0x01,
        
        ChainTooLowException = 0x02,

        // Used by ValidatingActionRenderer<T> (i.e., --strict-rendering):
        InvalidRenderException = 0x03,
    }
}
