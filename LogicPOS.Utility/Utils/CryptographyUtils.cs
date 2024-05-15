using System;
using System.Security.Cryptography;
using System.Text;

namespace LogicPOS.Utility
{
    public static class CryptographyUtils
    {
        private static readonly int _saltLength = 6;
        private static readonly string _delimiter = "*";
        private const string _key = "6f6a40b4419af34d562a95a1a7c5306d7b03";

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


        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            return Encrypt(toEncrypt, useHashing, _key);
        }

        public static string Encrypt(string toEncrypt, bool useHashing, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            return Decrypt(cipherString, useHashing, _key);
        }

        public static string Decrypt(string cipherString, bool useHashing, string pKey)
        {
            string result = cipherString;

            if (cipherString == null) return result;

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(pKey));
                hashmd5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(pKey);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            result = Encoding.UTF8.GetString(resultArray);

            return result;
        }
    }
}
