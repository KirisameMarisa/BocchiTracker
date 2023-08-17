using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IPasswordService
    {
        string Encrypy(string inText);

        string Decrypy(string inCipherText);
    }

    public class PasswordService : IPasswordService
    {
        private string? _macAddress;

        public PasswordService(IMacAddressProvider inMacAddressProvider)
        {
            var adresses = inMacAddressProvider.GetMacAddresses();
            if (adresses.Count > 0)
            {
                _macAddress = adresses[0];
            }
        }

        public string Encrypy(string inText)
        {
            if (string.IsNullOrEmpty(_macAddress))
                return string.Empty;

            byte[] encrypted;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateKey(_macAddress);
            aes.IV = GenerateIV(_macAddress);

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

        public string Decrypy(string inCipherText)
        {
            if (string.IsNullOrEmpty(_macAddress))
                return string.Empty;

            string? plaintext = null;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateKey(_macAddress);
            aes.IV = GenerateIV(_macAddress);

            try
            {
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
            catch
            {
                return string.Empty;
            }
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
