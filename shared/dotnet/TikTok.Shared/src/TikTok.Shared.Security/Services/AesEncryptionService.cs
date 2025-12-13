using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TikTok.Shared.Security.Interfaces;
using TikTok.Shared.Security.Models;

namespace TikTok.Shared.Security.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly EncryptionOptions _options;

        public AesEncryptionService(IOptions<EncryptionOptions> options)
        {
            _options = options.Value;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Plain text cannot be empty.", nameof(plainText));

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_options.Key);
            aes.IV = Encoding.UTF8.GetBytes(_options.IV);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                throw new ArgumentException("Cipher text cannot be empty.", nameof(cipherText));

            try
            {
                var buffer = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(_options.Key);
                aes.IV = Encoding.UTF8.GetBytes(_options.IV);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using var msDecrypt = new MemoryStream(buffer);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
            catch
            {
                throw new CryptographicException("Failed to decrypt the cipher text.");
            }
        }
    }
}
