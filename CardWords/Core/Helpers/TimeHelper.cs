using System;

namespace Reminder.Core.Helpers
{
    public static class TimeHelper
    {
        private const string hour = "ч";
        private const string minutes = "мин";
        private const string seconds = "c";

        public static string GetTime(TimeSpan timeLeft)
        {
            var absTime = GetAbsTime(timeLeft);

            var result = GetTimeInternal(absTime);

            return IsNegate(timeLeft) ? $"- {result}" : result;
        }

        private static TimeSpan GetAbsTime(TimeSpan timeLeft)
        {
            return IsNegate(timeLeft) ? timeLeft.Negate() : timeLeft;
        }

        private static bool IsNegate(TimeSpan timeLeft)
        {
            return timeLeft.TotalSeconds < 0;
        }

        private static string GetTimeInternal(TimeSpan timeLeft)
        {
            if (timeLeft.TotalSeconds < 60)
            {
                return GetSeconds(timeLeft);
            }

            if (timeLeft.TotalMinutes < 60)
            {
                return GetMinutes(timeLeft);
            }

            return GetHours(timeLeft);
        }

        private static string GetHours(TimeSpan timeLeft)
        {
            if(timeLeft.Minutes == 0)
            {
                return $"{timeLeft.Hours} {hour}";
            }

            return $"{timeLeft.Hours} {hour} {timeLeft.Minutes} {minutes}";
        }

        private static string GetMinutes(TimeSpan timeLeft)
        {
            return $"{timeLeft.Minutes} {minutes}";
        }

        private static string GetSeconds(TimeSpan timeLeft)
        {
            return $"{timeLeft.Seconds} {seconds}";
        }

        public static bool IsTimeStringChanged(TimeSpan timeLeft, TimeSpan newTime)
        {
            if(timeLeft == newTime)
            {
                return false;
            }

            var absTime = GetAbsTime(timeLeft);
            var newAbsTime = GetAbsTime(newTime);

            if(newAbsTime.TotalSeconds < 60)
            {
                return true;
            }

            if(absTime.Hours == newAbsTime.Hours && absTime.Minutes == newAbsTime.Minutes)
            {
                return false;
            }

            return true;
        }
    }
}
