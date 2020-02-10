using FasTnT.Domain.Model.Subscriptions;
using System;

namespace FasTnT.Domain.Subscriptions
{
    public class SubscriptionSchedule
    {
        private readonly ScheduleEntry _seconds, _minutes, _hours, _dayOfMonth, _month, _dayOfWeek;

        public SubscriptionSchedule(QuerySchedule schedule)
        {
            schedule = schedule ?? new QuerySchedule();

            _seconds = ScheduleEntry.Parse(schedule.Second, 0, 59);
            _minutes = ScheduleEntry.Parse(schedule.Minute, 0, 59);
            _hours = ScheduleEntry.Parse(schedule.Hour, 0, 23);
            _dayOfMonth = ScheduleEntry.Parse(schedule.DayOfMonth, 1, 31);
            _month = ScheduleEntry.Parse(schedule.Month, 1, 12);
            _dayOfWeek = ScheduleEntry.Parse(schedule.DayOfWeek, 1, 7);
        }

        public static bool IsValid(Subscription request)
        {
            try
            {
                new SubscriptionSchedule(request.Schedule);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public virtual DateTime GetNextOccurence(DateTime startDate)
        {
            var tentative = SetMonth(SetDayOfMonth(SetHours(SetMinutes(SetSeconds(startDate.AddSeconds(1)))))); // Parse from the next second

            if (!_dayOfWeek.HasValue(1 + (int)tentative.DayOfWeek))
            {
                return GetNextOccurence(new DateTime(tentative.Year, tentative.Month, tentative.Day, 23, 59, 59));
            }

            return tentative;
        }

        private DateTime SetMinutes(DateTime tentative)
        {
            if (!_minutes.HasValue(tentative.Minute)) tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, tentative.Hour, Math.Max(tentative.Minute, _minutes.Min), _seconds.Min);
            while (!_minutes.HasValue(tentative.Minute)) tentative = tentative.AddMinutes(1);

            return tentative;
        }

        private DateTime SetHours(DateTime tentative)
        {
            if (!_hours.HasValue(tentative.Hour)) tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, Math.Max(tentative.Hour, _hours.Min), _minutes.Min, _seconds.Min);
            while (!_hours.HasValue(tentative.Hour)) tentative = tentative.AddHours(1);

            return tentative;
        }

        private DateTime SetDayOfMonth(DateTime tentative)
        {
            if (!_dayOfMonth.HasValue(tentative.Day)) tentative = new DateTime(tentative.Year, tentative.Month, Math.Max(tentative.Day, _dayOfMonth.Min), _hours.Min, _minutes.Min, _seconds.Min);
            while (!_dayOfMonth.HasValue(tentative.Day)) tentative = tentative.AddDays(1);

            return tentative;
        }

        private DateTime SetMonth(DateTime tentative)
        {
            if (!_month.HasValue(tentative.Month)) tentative = new DateTime(tentative.Year, Math.Max(tentative.Month, _month.Min), _dayOfMonth.Min, _hours.Min, _hours.Min, _minutes.Min, _seconds.Min);
            while (!_month.HasValue(tentative.Month)) tentative = tentative.AddMonths(1);

            return tentative;
        }

        private DateTime SetSeconds(DateTime tentative)
        {
            while (!_seconds.HasValue(tentative.Second)) tentative = tentative.AddSeconds(1);
            return tentative;
        }
    }
}
