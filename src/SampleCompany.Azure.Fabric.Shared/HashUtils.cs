using System;
using System.Security.Cryptography;
using System.Text;

namespace SampleCompany.Azure.Fabric.Shared
{
    public static class HashUtils
    {
        public static long GetLongHashCode(string stringInput)
        {
            byte[] byteContents = Encoding.Unicode.GetBytes(stringInput);
            using (var hash = new SHA1CryptoServiceProvider())
            {
                byte[] hashText = hash.ComputeHash(byteContents);
                return BitConverter.ToInt64(hashText, 0) ^ BitConverter.ToInt64(hashText, 7);
            }
        }

        public static int GetIntHashCode(string stringInput)
        {
            return (int)GetLongHashCode(stringInput);
        }
    }
}
