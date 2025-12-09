using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace TikTok.Shared.Common.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 4; // Argon2 iterations
        private const int MemorySize = 65536; // 64 MB (64 * 1024 KB)
        private const int DegreeOfParallelism = 1;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            // Generate salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Hash password with Argon2id
            byte[] hash = HashPasswordWithArgon2(password, salt);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Hashed password cannot be empty.", nameof(hashedPassword));

            try
            {
                // Decode base64
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                if (hashBytes.Length != SaltSize + HashSize)
                    return false;

                // Extract salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Extract hash
                byte[] storedHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

                // Hash the input password with the same salt
                byte[] computedHash = HashPasswordWithArgon2(password, salt);

                // Compare hashes
                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] HashPasswordWithArgon2(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                MemorySize = MemorySize,
                Iterations = Iterations
            };

            return argon2.GetBytes(HashSize);
        }
    }
}
