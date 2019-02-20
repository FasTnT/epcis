using FasTnT.Model.Queries;

namespace FasTnT.Model.Subscriptions
{
    public class UnsubscribeRequest : EpcisQuery
    {
        public string SubscriptionId { get; set; }
    }
}
