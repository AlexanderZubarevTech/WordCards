using System;

namespace WordCards.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndDay(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59);
        }

        public static DateTime BeginDay(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day);
        }

        public static string ToSqlString(this DateTime dateTime)
        {
            return $"'{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}'";
        }
    }
}
