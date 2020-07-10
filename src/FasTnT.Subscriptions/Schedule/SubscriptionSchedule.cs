using FasTnT.Domain.Model.Subscriptions;
using System;

namespace FasTnT.Subscriptions.Schedule
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
            if (!_minutes.HasValue(tentative.Minute))
            {
                tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, tentative.Hour, Math.Max(tentative.Minute, _minutes.Min), _seconds.Min);
            }

            return GetNextTentative(tentative, x => x.Minute, x => x.AddMinutes(1), _minutes);
        }

        private DateTime SetHours(DateTime tentative)
        {
            if (!_hours.HasValue(tentative.Hour))
            {
                tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, Math.Max(tentative.Hour, _hours.Min), _minutes.Min, _seconds.Min);
            }

            return GetNextTentative(tentative, x => x.Hour, x => x.AddHours(1), _hours);
        }

        private DateTime SetDayOfMonth(DateTime tentative)
        {
            if (!_dayOfMonth.HasValue(tentative.Day))
            {
                tentative = new DateTime(tentative.Year, tentative.Month, Math.Max(tentative.Day, _dayOfMonth.Min), _hours.Min, _minutes.Min, _seconds.Min);
            }

            return GetNextTentative(tentative, x => x.Day, x => x.AddDays(1), _dayOfMonth);
        }

        private DateTime SetMonth(DateTime tentative)
        {
            if (!_month.HasValue(tentative.Month))
            {
                tentative = new DateTime(tentative.Year, Math.Max(tentative.Month, _month.Min), _dayOfMonth.Min, _hours.Min, _hours.Min, _minutes.Min, _seconds.Min);
            }

            return GetNextTentative(tentative, x => x.Month, x => x.AddMonths(1), _month);
        }

        private DateTime SetSeconds(DateTime tentative)
        {
            return GetNextTentative(tentative, x => x.Second, x => x.AddSeconds(1), _seconds);
        }

        private static DateTime GetNextTentative(DateTime tentative, Func<DateTime, int> selector, Func<DateTime, DateTime> increment, ScheduleEntry entry)
        {
            while (!entry.HasValue(selector(tentative)))
            {
                tentative = increment(tentative);
            }

            return tentative;
        }
    }
}
