using System;
using System.Security.Cryptography;
using System.Text;
using SIS.Framework.Services.Contracts;

namespace SIS.Framework.Services
{
    public class HashService : IHashService
    {
        public string Hash(string stringToHash)
        {
            stringToHash = stringToHash + "a12bdg3u7d6jmb67a0p#ah";

            using (var sha256 = SHA256.Create())
            { 
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hash;
            }
        }
    }
}
