using System.Collections.Generic;
using MagicOnion;

namespace LibUnity.RPC.Services
{
    public interface IBlockChainService : IService<IBlockChainService>
    {
        UnaryResult<bool> PutTransaction(byte[] txBytes);

        UnaryResult<long> GetNextTxNonce(byte[] addressBytes);

        UnaryResult<byte[]> GetState(byte[] addressBytes);

        UnaryResult<byte[]> GetBalance(byte[] addressBytes, byte[] currencyBytes);
        
        UnaryResult<byte[]> GetTip();

        UnaryResult<bool> SetAddressesToSubscribe(IEnumerable<byte[]> addressesBytes);

        UnaryResult<bool> IsTransactionStaged(byte[] txidBytes);

        UnaryResult<bool> ReportException(string code, string message);
    }
}
