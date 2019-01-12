using System;

namespace FasTnT.Model.Subscriptions
{
    public class SubscriptionControls
    {
        public Guid SubscriptionId { get; set; }
        public QuerySchedule Schedule { get; set; }
        public string Trigger { get; set; }
        public DateTime InitialRecordTime { get; set; }
        public bool ReportIfEmpty { get; set; }
    }
}
