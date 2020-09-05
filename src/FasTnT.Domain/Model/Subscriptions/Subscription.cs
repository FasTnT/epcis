using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;

namespace FasTnT.Domain.Model.Subscriptions
{
    public class Subscription
    {
        public bool Active { get; set; } = true;
        public string SubscriptionId { get; set; }
        public string QueryName { get; set; }
        public List<QueryParameter> Parameters { get; set; } = new List<QueryParameter>();
        public bool ReportIfEmpty { get; set; }
        public string Destination { get; set; }
        public QuerySchedule Schedule { get; set; }
        public string Trigger { get; set; }
        public DateTime? InitialRecordTime { get; set; }
    }
}
