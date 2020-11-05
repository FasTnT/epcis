using System;

namespace FasTnT.Subscriptions.Utils
{
    public static class DateTimeExtensions
    {
        public static TimeSpan GetDifferenceTime(this DateTime target, DateTime source)
        {
            return (target - source) > TimeSpan.Zero
                ? target - source
                : TimeSpan.Zero;
        }
    }
}
