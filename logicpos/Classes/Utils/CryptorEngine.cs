using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

//ThirdParty Lib - Adapted to LogicPos

namespace logicpos
{
    public class CryptorEngine
    {
        //Default Key
        private const string _key = "6f6a40b4419af34d562a95a1a7c5306d7b03";

        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
        /// </summary>
        /// <param name="pToEncrypt">string to be encrypted</param>
        /// <param name="pUseHashing">use hashing? send to for extra secirity</param>
        /// <returns></returns>
        public static string Encrypt(string pToEncrypt, bool pUseHashing)
        {
            return Encrypt(pToEncrypt, pUseHashing, _key);
        }

        public static string Encrypt(string pToEncrypt, bool pUseHashing, string pKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pToEncrypt);

            if (pUseHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(pKey));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(pKey);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="pCipherString">encrypted string</param>
        /// <param name="pUseHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        /// 
        public static string Decrypt(string pCipherString, bool pUseHashing)
        {
            return Decrypt(pCipherString, pUseHashing, _key);
        }

        public static string Decrypt(string pCipherString, bool pUseHashing, string pKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(pCipherString);
            
            if (pUseHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(pKey));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                        
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
