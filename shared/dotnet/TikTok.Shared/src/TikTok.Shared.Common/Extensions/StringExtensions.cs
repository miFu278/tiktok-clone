using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TikTok.Shared.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Convert Vietnamese characters to non-accented
            text = RemoveVietnameseTones(text);

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Remove special characters
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Replace multiple spaces/hyphens with single hyphen
            text = Regex.Replace(text, @"[\s-]+", "-");

            // Trim hyphens from start and end
            text = text.Trim('-');

            return text;
        }

        public static string RemoveVietnameseTones(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var vietnameseChars = new[]
            {
                "aàảãáạăằẳẵắặâầẩẫấậ",
                "AÀẢÃÁẠĂẰẲẴẮẶÂẦẨẪẤẬ",
                "dđ", "DĐ",
                "eèẻẽéẹêềểễếệ",
                "EÈẺẼÉẸÊỀỂỄẾỆ",
                "iìỉĩíị",
                "IÌỈĨÍỊ",
                "oòỏõóọôồổỗốộơờởỡớợ",
                "OÒỎÕÓỌÔỒỔỖỐỘƠỜỞỠỚỢ",
                "uùủũúụưừửữứự",
                "UÙỦŨÚỤƯỪỬỮỨỰ",
                "yỳỷỹýỵ",
                "YỲỶỸÝỴ"
            };

            for (int i = 0; i < vietnameseChars.Length; i++)
            {
                char replaceChar = vietnameseChars[i][0];
                for (int j = 1; j < vietnameseChars[i].Length; j++)
                {
                    text = text.Replace(vietnameseChars[i][j], replaceChar);
                }
            }

            return text;
        }

        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + suffix;
        }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static string ToBase64(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string base64Text)
        {
            if (string.IsNullOrWhiteSpace(base64Text))
                return string.Empty;

            var bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
