namespace LogicPOS.Persistence.Services;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EncryptionService : ValueConverter<string, string>
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); 

    public EncryptionService() : base(
        v => Encrypt(v),
        v => Decrypt(v)
    )
    {
        
    }

    private static string Encrypt(string plainText)
    {
        if(string.IsNullOrWhiteSpace(plainText))
        {
            return plainText;
        }
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private static string Decrypt(string cipherText)
    {
        if(string.IsNullOrWhiteSpace(cipherText))
        {
            return cipherText;
        }

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
        using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
   
}
