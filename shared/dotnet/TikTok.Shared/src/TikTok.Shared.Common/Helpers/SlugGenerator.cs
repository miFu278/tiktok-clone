using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TikTok.Shared.Common.Helpers
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string text, bool lowercase = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Remove Vietnamese tones
            text = RemoveVietnameseTones(text);

            // Convert to lowercase if needed
            if (lowercase)
                text = text.ToLowerInvariant();

            // Remove special characters, keep only alphanumeric, spaces, and hyphens
            text = Regex.Replace(text, @"[^a-zA-Z0-9\s-]", "");

            // Replace multiple spaces or hyphens with single hyphen
            text = Regex.Replace(text, @"[\s-]+", "-");

            // Trim hyphens from start and end
            text = text.Trim('-');

            return text;
        }

        public static string GenerateUniqueSlug(string text, Func<string, bool> slugExists)
        {
            var baseSlug = GenerateSlug(text);
            var slug = baseSlug;
            var counter = 1;

            while (slugExists(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public static string GenerateSlugWithId(string text, string id)
        {
            var slug = GenerateSlug(text);
            return string.IsNullOrWhiteSpace(slug) ? id : $"{slug}-{id}";
        }

        private static string RemoveVietnameseTones(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var vietnameseChars = new Dictionary<char, string>
            {
                {'á', "a"}, {'à', "a"}, {'ả', "a"}, {'ã', "a"}, {'ạ', "a"},
                {'ă', "a"}, {'ắ', "a"}, {'ằ', "a"}, {'ẳ', "a"}, {'ẵ', "a"}, {'ặ', "a"},
                {'â', "a"}, {'ấ', "a"}, {'ầ', "a"}, {'ẩ', "a"}, {'ẫ', "a"}, {'ậ', "a"},
                {'Á', "A"}, {'À', "A"}, {'Ả', "A"}, {'Ã', "A"}, {'Ạ', "A"},
                {'Ă', "A"}, {'Ắ', "A"}, {'Ằ', "A"}, {'Ẳ', "A"}, {'Ẵ', "A"}, {'Ặ', "A"},
                {'Â', "A"}, {'Ấ', "A"}, {'Ầ', "A"}, {'Ẩ', "A"}, {'Ẫ', "A"}, {'Ậ', "A"},
                {'đ', "d"}, {'Đ', "D"},
                {'é', "e"}, {'è', "e"}, {'ẻ', "e"}, {'ẽ', "e"}, {'ẹ', "e"},
                {'ê', "e"}, {'ế', "e"}, {'ề', "e"}, {'ể', "e"}, {'ễ', "e"}, {'ệ', "e"},
                {'É', "E"}, {'È', "E"}, {'Ẻ', "E"}, {'Ẽ', "E"}, {'Ẹ', "E"},
                {'Ê', "E"}, {'Ế', "E"}, {'Ề', "E"}, {'Ể', "E"}, {'Ễ', "E"}, {'Ệ', "E"},
                {'í', "i"}, {'ì', "i"}, {'ỉ', "i"}, {'ĩ', "i"}, {'ị', "i"},
                {'Í', "I"}, {'Ì', "I"}, {'Ỉ', "I"}, {'Ĩ', "I"}, {'Ị', "I"},
                {'ó', "o"}, {'ò', "o"}, {'ỏ', "o"}, {'õ', "o"}, {'ọ', "o"},
                {'ô', "o"}, {'ố', "o"}, {'ồ', "o"}, {'ổ', "o"}, {'ỗ', "o"}, {'ộ', "o"},
                {'ơ', "o"}, {'ớ', "o"}, {'ờ', "o"}, {'ở', "o"}, {'ỡ', "o"}, {'ợ', "o"},
                {'Ó', "O"}, {'Ò', "O"}, {'Ỏ', "O"}, {'Õ', "O"}, {'Ọ', "O"},
                {'Ô', "O"}, {'Ố', "O"}, {'Ồ', "O"}, {'Ổ', "O"}, {'Ỗ', "O"}, {'Ộ', "O"},
                {'Ơ', "O"}, {'Ớ', "O"}, {'Ờ', "O"}, {'Ở', "O"}, {'Ỡ', "O"}, {'Ợ', "O"},
                {'ú', "u"}, {'ù', "u"}, {'ủ', "u"}, {'ũ', "u"}, {'ụ', "u"},
                {'ư', "u"}, {'ứ', "u"}, {'ừ', "u"}, {'ử', "u"}, {'ữ', "u"}, {'ự', "u"},
                {'Ú', "U"}, {'Ù', "U"}, {'Ủ', "U"}, {'Ũ', "U"}, {'Ụ', "U"},
                {'Ư', "U"}, {'Ứ', "U"}, {'Ừ', "U"}, {'Ử', "U"}, {'Ữ', "U"}, {'Ự', "U"},
                {'ý', "y"}, {'ỳ', "y"}, {'ỷ', "y"}, {'ỹ', "y"}, {'ỵ', "y"},
                {'Ý', "Y"}, {'Ỳ', "Y"}, {'Ỷ', "Y"}, {'Ỹ', "Y"}, {'Ỵ', "Y"}
            };

            var result = new StringBuilder();
            foreach (char c in text)
            {
                if (vietnameseChars.TryGetValue(c, out var replacement))
                    result.Append(replacement);
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}
