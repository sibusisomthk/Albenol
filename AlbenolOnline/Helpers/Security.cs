using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbenolOnline.Helpers
{
    public class Security
    {
        public static string HashSensitiveData(string unHashed)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(unHashed);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }
        public static bool CompareHashedData(string hashed, string userinput)
        {
            string hashedUserInput = HashSensitiveData(userinput);
            if (hashed.Equals(hashedUserInput))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
