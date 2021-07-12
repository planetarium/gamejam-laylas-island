using System.Security.Cryptography;
using System.Text;
using Libplanet;

namespace LaylasIsland.Backend.Extensions
{
    public static class AddressExtensions
    {
        public static Address Derive(this Address address, byte[] key)
        {
            var bytes = address.ToByteArray();
            byte[] hashed;

            using (var hmac = new HMACSHA1(key))
            {
                hashed = hmac.ComputeHash(bytes);
            }

            return new Address(hashed);
        }

        public static Address Derive(this Address address, string key) => address.Derive(Encoding.UTF8.GetBytes(key));
    }
}
