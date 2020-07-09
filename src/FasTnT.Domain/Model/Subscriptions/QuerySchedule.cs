﻿using System.Text.RegularExpressions;

namespace FasTnT.Domain.Model.Subscriptions
{
    public class QuerySchedule
    {
        public string Second { get; set; }
        public string Minute { get; set; }
        public string Hour { get; set; }
        public string DayOfMonth { get; set; }
        public string Month { get; set; }
        public string DayOfWeek { get; set; }

        public static bool IsValid(QuerySchedule schedule)
        {
            return SecondRegex.IsMatch(schedule.Second)
                && MinuteRegex.IsMatch(schedule.Minute)
                && HourRegex.IsMatch(schedule.Hour)
                && DayOfMonthRegex.IsMatch(schedule.DayOfMonth)
                && MonthRegex.IsMatch(schedule.Month)
                && DayOfWeekRegex.IsMatch(schedule.DayOfWeek);
        }

        private readonly static Regex SecondRegex = BuildRegex("[0-9]|([0-5][0-9])");
        private readonly static Regex MinuteRegex = BuildRegex("[0-9]|([0-5][0-9])");
        private readonly static Regex HourRegex = BuildRegex("[0-9]|(1[0-9])|(2[0-3])");
        private readonly static Regex DayOfMonthRegex = BuildRegex("[1-9]|([1-2][0-9])|(3[0-1])");
        private readonly static Regex MonthRegex = BuildRegex("[1-9]|([1][0-2])");
        private readonly static Regex DayOfWeekRegex = BuildRegex("[1-7]");
    
        private static Regex BuildRegex(string range)
        {
            var element = $@"(({range})|(\[({range})\-({range})\]))";

            return new Regex($@"^({element}((\,{element})+)?)?$");
        }
    }
}
