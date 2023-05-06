using System;

namespace CardWords.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndDay(this DateTime datetime) 
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59);
        }

        public static DateTime StartDay(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day);
        }
    }
}
