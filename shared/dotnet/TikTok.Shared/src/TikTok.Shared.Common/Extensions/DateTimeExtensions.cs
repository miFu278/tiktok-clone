namespace TikTok.Shared.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(this long unixTimestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimestamp);
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || 
                   dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        public static int GetAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            
            if (birthDate.Date > today.AddYears(-age))
                age--;
            
            return age;
        }

        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();

            if (timeSpan.TotalSeconds < 60)
                return "just now";
            
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
            
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
            
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
            
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
            
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";
            
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago";
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return dateTime.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
        }

        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
        }
    }
}
