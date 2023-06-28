using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BocchiTracker.ProjectConfig;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Slack.NetStandard;
using static System.Net.Mime.MediaTypeNames;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IPasswordService
    {
        string Encrypy(string inText, string inMacAddress);

        string Decrypy(string inCipherText, string inMacAddress);
    }

    public class PasswordService : IPasswordService
    {
        public string Encrypy(string inText, string inMacAddress)
        {
            byte[] encrypted;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateKey(inMacAddress);
            aes.IV = GenerateIV(inMacAddress);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(inText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypy(string inCipherText, string inMacAddress)
        {
            string? plaintext = null;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateKey(inMacAddress);
            aes.IV = GenerateIV(inMacAddress);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(inCipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        private byte[] GenerateIV(string inMacAddress)
        {
            byte[] mac_bytes = Encoding.UTF8.GetBytes(inMacAddress.Replace(":", "").ToLower());
            byte[] iv = new byte[16];

            Array.Copy(mac_bytes, iv, Math.Min(mac_bytes.Length, iv.Length));
            return iv;
        }

        private byte[] GenerateKey(string inMacAddress)
        {
            byte[] mac_bytes = Encoding.UTF8.GetBytes(inMacAddress.Replace(":", "").ToLower());
            byte[] key = new byte[32];

            for (int i = 0; i < key.Length; i++)
                key[i] = mac_bytes[i % mac_bytes.Length];
            return key;
        }
    }
}
