using System.Security.Cryptography;
using System.Text;

namespace ProjectBank.Infrastructure
{
    //Responsible for computing hash codes
    public class HashBuilder
    {
        public static string HashString(string str, HashAlgorithm Hasher)
        {
            byte[] bytes = Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            var code = ToString(bytes);
            return code;
        }

        private static string ToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}