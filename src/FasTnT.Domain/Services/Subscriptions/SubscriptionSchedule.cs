using FasTnT.Model.Subscriptions;
using System;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class SubscriptionSchedule
    {
        private ScheduleEntry _seconds, _minutes, _hours, _dayOfMonth, _month, _dayOfWeek;

        public SubscriptionSchedule(Subscription subscription)
        {
            _seconds = ScheduleEntry.Parse(subscription.Schedule?.Second ?? "0", 0, 59);
            _minutes = ScheduleEntry.Parse(subscription.Schedule?.Minute ?? "", 0, 59);
            _hours = ScheduleEntry.Parse(subscription.Schedule?.Hour ?? "", 0, 23);
            _dayOfMonth = ScheduleEntry.Parse(subscription.Schedule?.DayOfMonth ?? "", 1, 31);
            _month = ScheduleEntry.Parse(subscription.Schedule?.Month ?? "", 1, 12);
            _dayOfWeek = ScheduleEntry.Parse(subscription.Schedule?.DayOfWeek ?? "", 1, 7);
        }

        public virtual DateTime GetNextOccurence(DateTime startDate)
        {
            var tentative = startDate.AddSeconds(1); // Parse from the next second

            while (!_seconds.HasValue(tentative.Second)) tentative = tentative.AddSeconds(1);
            while (!_minutes.HasValue(tentative.Minute)) tentative = tentative.AddMinutes(1);
            while (!_hours.HasValue(tentative.Hour)) tentative = tentative.AddHours(1);
            while (!_dayOfMonth.HasValue(tentative.Day)) tentative = tentative.AddDays(1);
            while (!_month.HasValue(tentative.Month)) tentative = tentative.AddMonths(1);

            if (!_dayOfWeek.HasValue(1 + (int)tentative.DayOfWeek))
            {
                // Try again from next day.
                return GetNextOccurence(new DateTime(tentative.Year, tentative.Month, tentative.Day, 23, 59, 59));
            }

            return tentative;
        }
    }
}
