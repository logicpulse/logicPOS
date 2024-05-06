using System.Security.Cryptography;
using System;

namespace LogicPOS.Utility
{
    public static class CryptographyUtils
    {
        private static readonly int _saltLength = 6;
        private static readonly string _delimiter = "*";

        public static string SaltString(string pSaltString, string pSalt)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pSalt + pSaltString)));
        }

        public static string GenerateSaltedString(string pSaltString)
        {
            if (string.IsNullOrEmpty(pSaltString))
            {
                return pSaltString;
            }

            byte[] randomSalt = new byte[_saltLength];
            new RNGCryptoServiceProvider().GetBytes(randomSalt);
            string salt = Convert.ToBase64String(randomSalt);
            return salt + _delimiter + SaltString(pSaltString, salt);
        }

        public static bool ValidateSaltedString(string pSaltedString, string pPlainTextString)
        {
            if (string.IsNullOrEmpty(pSaltedString))
                return string.IsNullOrEmpty(pPlainTextString);
            if (string.IsNullOrEmpty(pPlainTextString))
            {
                return false;
            }
            int delimPos = pSaltedString.IndexOf(_delimiter);
            if (delimPos <= 0)
            {
                return pSaltedString.Equals(pPlainTextString);
            }
            else
            {
                string calculatedSaltedString = SaltString(pPlainTextString, pSaltedString.Substring(0, delimPos));
                string expectedSaltedString = pSaltedString.Substring(delimPos + _delimiter.Length);
                if (expectedSaltedString.Equals(calculatedSaltedString))
                {
                    return true;
                }
                return expectedSaltedString.Equals(SaltString(pPlainTextString, "System.Byte[]"));
            }
        }
    }
}
