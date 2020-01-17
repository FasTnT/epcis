using FasTnT.Model.Queries;

namespace FasTnT.Domain.Model.Subscriptions
{
    public class Subscription
    {
        public int? Id { get; set; }
        public string SubscriptionId { get; set; }
        public string QueryName { get; set; }
        public QueryParameter[] Parameters { get; set; }
        public bool ReportIfEmpty { get; set; }
        public string Destination { get; set; }
    }
}
