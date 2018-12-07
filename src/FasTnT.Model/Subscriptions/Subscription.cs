using FasTnT.Model.Queries.PredefinedQueries.Parameters;
using System.Collections.Generic;

namespace FasTnT.Model.Subscriptions
{
    public class Subscription
    {
        public string QueryName { get; set; }
        public IList<SimpleEventQueryParameter> Params { get; set; }
        public string Destination { get; set; }
        public SubscriptionControls Controls { get; set; }
        public string SubscriptionId { get; set; }
    }
}
