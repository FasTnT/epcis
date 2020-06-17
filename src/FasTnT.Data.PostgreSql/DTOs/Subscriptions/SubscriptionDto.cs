using FasTnT.Domain.Model.Subscriptions;
using System;
using System.Security.Cryptography;

namespace FasTnT.Data.PostgreSql.DTOs.Subscriptions
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string SubscriptionId { get; set; }
        public string Trigger { get; set; }
        public DateTime? InitialRecordTime { get; set; }
        public bool ReportIfEmpty { get; set; }
        public string Second { get; set; }
        public string Minute { get; set; }
        public string Hour { get; set; }
        public string Month { get; set; }
        public string DayOfWeek { get; set; }
        public string DayOfMonth { get; set; }
        public string Destination { get; set; }
        public string QueryName { get; set; }

        public static SubscriptionDto Create(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Active = subscription.Active,
                SubscriptionId = subscription.SubscriptionId,
                Trigger = subscription.Trigger,
                InitialRecordTime = subscription.InitialRecordTime,
                ReportIfEmpty = subscription.ReportIfEmpty,
                Second = subscription.Schedule?.Second,
                Minute = subscription.Schedule?.Minute,
                Hour = subscription.Schedule?.Hour,
                DayOfWeek = subscription.Schedule?.DayOfWeek,
                DayOfMonth = subscription.Schedule?.DayOfMonth,
                Destination = subscription.Destination,
                QueryName = subscription.QueryName
            };
        }

        internal Subscription ToSubscription()
        {
            return new Subscription
            {
                Active = Active,
                SubscriptionId = SubscriptionId,
                Destination = Destination,
                InitialRecordTime = InitialRecordTime,
                QueryName = QueryName,
                ReportIfEmpty = ReportIfEmpty,
                Trigger = Trigger,
                Schedule = FormatSchedule()
            };
        }

        private QuerySchedule FormatSchedule()
        {
            return string.IsNullOrEmpty(Trigger)
                ? new QuerySchedule
                {
                    Second = Second,
                    Minute = Minute,
                    Hour = Hour,
                    Month = Month,
                    DayOfWeek = DayOfWeek,
                    DayOfMonth =DayOfMonth
                }
                : null;
        }
    }
}
