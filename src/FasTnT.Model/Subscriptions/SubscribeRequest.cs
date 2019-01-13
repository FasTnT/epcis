using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;

namespace FasTnT.Model.Subscriptions
{
    public class Subscription : SubscriptionRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QueryName { get; set; }
        public string Destination { get; set; }
        public string SubscriptionId { get; set; }
        public QuerySchedule Schedule { get; set; }
        public string Trigger { get; set; }
        public DateTime InitialRecordTime { get; set; }
        public bool ReportIfEmpty { get; set; }
        public IEnumerable<QueryParameter> Params { get; set; }
    }
}
