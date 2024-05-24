using LogicPOS.Globalization;
using LogicPOS.Settings;
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

        public static string SignDataToSHA1Base64(string pPrivateKeyPT, string pPrivateKeyAO, string pEncryptData, bool pDebug = false)
        {
            if (GeneralSettings.Settings["cultureFinancialRules"] == "pt-AO")
            {
                return SHA1SignMessage(pPrivateKeyAO, Encoding.UTF8.GetBytes(pEncryptData), pDebug);
            }

            return SHA1SignMessage(pPrivateKeyPT, Encoding.UTF8.GetBytes(pEncryptData), pDebug);
        }

        private static string SHA1SignMessage(string pPrivateKey, byte[] pMessage, bool pDebug = false)
        {
            String tempPath = Convert.ToString(PathsSettings.TempFolderLocation);

            //Init RSACryptoServiceProvider with Key
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();

            rsaCryptoServiceProvider.FromXmlString(pPrivateKey);

            //SHA1 Sign 
            byte[] signature = rsaCryptoServiceProvider.SignData(pMessage, CryptoConfig.MapNameToOID("SHA1"));
            string signatureBase64 = Convert.ToBase64String(signature);

            if (pDebug)
            {
                System.IO.File.WriteAllBytes(tempPath + "encrypt.txt", pMessage);
                System.IO.File.WriteAllBytes(tempPath + "encrypted.sha1", signature);
                System.IO.File.WriteAllText(tempPath + "encrypted.b64", signatureBase64);
            }

            return signatureBase64;
        }

        //GenDocumentHash4Chars
        //Get 1º, 11º, 21º, 31º From Hash, Required for Printed Versions (Reports and Tickets)
        public static string GetDocumentHash4Chars(string pHash)
        {
            // Protection In case of bad hash, ex when we dont have SoftwareVendorPlugin Registered
            if (string.IsNullOrEmpty(pHash))
            {
                throw new Exception(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document_bad_hash_detected"));
            }
            else
            {
                string a = pHash.Substring(1 - 1, 1);
                string b = pHash.Substring(11 - 1, 1);
                string c = pHash.Substring(21 - 1, 1);
                string d = pHash.Substring(31 - 1, 1);

                //_logger.Debug(string.Format("pHash: [{0}] [{1}][{2}][{3}][{4}]", pHash, a, b, c, d));
                //Ex.: Result [wESm]
                //wQ5dp/AesYEgM9QFlh8aSyfIcpJIDnm+Z8cr4PNsmF7AoxIR9+EU8vIq2PDXE7aIMYH0j.....
                //[w]:w
                //[E]:wQ5dp/AesYE
                //[S]:wQ5dp/AesYEgM9QFlh8aS
                //[m]_wQ5dp/AesYEgM9QFlh8aSyfIcpJIDnm

                return string.Format("{0}{1}{2}{3}", a, b, c, d);
            }
        }
    }
}
