using System.ComponentModel;
using System.Reflection;

namespace TikTok.Shared.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            
            if (field == null)
                return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            
            return attribute?.Description ?? value.ToString();
        }

        public static T? GetAttribute<T>(this Enum value) where T : Attribute
        {
            var field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<T>();
        }

        public static List<T> ToList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static Dictionary<int, string> ToDictionary<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(e => Convert.ToInt32(e), e => e.ToString());
        }

        public static Dictionary<int, string> ToDictionaryWithDescription<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(e => Convert.ToInt32(e), e => (e as Enum)!.GetDescription());
        }
    }
}
