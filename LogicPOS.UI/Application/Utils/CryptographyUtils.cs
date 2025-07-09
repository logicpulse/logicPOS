using System;
using System.Security.Cryptography;
using System.Text;

namespace LogicPOS.Utility
{
    public static class CryptographyUtils
    {
        private const string _key = "6f6a40b4419af34d562a95a1a7c5306d7b03";

     
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
