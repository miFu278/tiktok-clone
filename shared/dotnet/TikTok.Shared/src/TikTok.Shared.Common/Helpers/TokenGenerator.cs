using System.Security.Cryptography;
using System.Text;

namespace TikTok.Shared.Common.Helpers
{
    public static class TokenGenerator
    {
        public static string GenerateRandomToken(int length = 32)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be greater than 0.", nameof(length));

            byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, length);
        }

        public static string GenerateSecureToken(int byteSize = 32)
        {
            if (byteSize <= 0)
                throw new ArgumentException("Byte size must be greater than 0.", nameof(byteSize));

            byte[] randomBytes = RandomNumberGenerator.GetBytes(byteSize);
            return Convert.ToBase64String(randomBytes);
        }

        public static string GenerateNumericCode(int digits = 6)
        {
            if (digits <= 0 || digits > 10)
                throw new ArgumentException("Digits must be between 1 and 10.", nameof(digits));

            int min = (int)Math.Pow(10, digits - 1);
            int max = (int)Math.Pow(10, digits) - 1;

            return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
        }

        public static string GenerateUrlSafeToken(int length = 32)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be greater than 0.", nameof(length));

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return result.ToString();
        }

        public static string GenerateRefreshToken()
        {
            return GenerateSecureToken(64);
        }

        public static string GenerateVerificationToken()
        {
            return GenerateUrlSafeToken(48);
        }

        public static string GenerateResetPasswordToken()
        {
            return GenerateUrlSafeToken(48);
        }

        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        public static string GenerateShortId(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
