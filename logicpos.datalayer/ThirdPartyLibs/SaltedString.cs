using System;
using System.Security.Cryptography;

namespace CryptographyUtils
{
    public class SaltedString
    {
        static int saltLength = 6;
        static string delim = "*";

        public static string SaltString(string pSaltString, string pSalt)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pSalt + pSaltString)));
        }

        public static string GenerateSaltedString(string pSaltString)
        {
            if (string.IsNullOrEmpty(pSaltString))
                return pSaltString;
            byte[] randomSalt = new byte[saltLength];
            new RNGCryptoServiceProvider().GetBytes(randomSalt);
            string salt = Convert.ToBase64String(randomSalt);
            return salt + delim + SaltString(pSaltString, salt);
        }

        public static bool ValidateSaltedString(string pSaltedString, string pPlainTextString)
        {
            if (string.IsNullOrEmpty(pSaltedString))
                return string.IsNullOrEmpty(pPlainTextString);
            if (string.IsNullOrEmpty(pPlainTextString))
            {
                return false;
            }
            int delimPos = pSaltedString.IndexOf(delim);
            if (delimPos <= 0)
            {
                return pSaltedString.Equals(pPlainTextString);
            }
            else
            {
                string calculatedSaltedString = SaltString(pPlainTextString, pSaltedString.Substring(0, delimPos));
                string expectedSaltedString = pSaltedString.Substring(delimPos + delim.Length);
                if (expectedSaltedString.Equals(calculatedSaltedString))
                {
                    return true;
                }
                return expectedSaltedString.Equals(SaltString(pPlainTextString, "System.Byte[]"));
            }
        }
    }
}