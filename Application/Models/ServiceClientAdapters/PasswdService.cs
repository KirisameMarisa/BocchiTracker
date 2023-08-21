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
        public string Encrypy(string inText)
        {
            byte[] encrypted;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateChiper(32);
            aes.IV = GenerateChiper(16);

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
            return $"{Convert.ToBase64String(aes.Key)}:{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(encrypted)}";
        }

        public string Decrypy(string inText)
        {
            var parts = inText.Split(":");
            if (parts.Length != 3)
                return string.Empty;
            
            string? plaintext = null;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(parts[0]);
            aes.IV = Convert.FromBase64String(parts[1]);
            byte[] password = Convert.FromBase64String(parts[2]);

            try
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(password))
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

        private byte[] GenerateChiper(int inSize)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var chiper = new byte[inSize];
                rng.GetBytes(chiper);
                return chiper;
            }
        }
    }
}
