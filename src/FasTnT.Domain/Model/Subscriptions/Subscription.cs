using FasTnT.Model.Queries;
using System;

namespace FasTnT.Domain.Model.Subscriptions
{
    public class Subscription
    {
        public int? Id { get; set; }
        public bool Active { get; set; } = true;
        public string SubscriptionId { get; set; }
        public string QueryName { get; set; }
        public QueryParameter[] Parameters { get; set; }
        public bool ReportIfEmpty { get; set; }
        public string Destination { get; set; }
        public QuerySchedule Schedule { get; set; }
        public string Trigger { get; set; }
        public DateTime? InitialRecordTime { get; set; }
    }
}
