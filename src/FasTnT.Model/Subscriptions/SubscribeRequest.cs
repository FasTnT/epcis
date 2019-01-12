using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;

namespace FasTnT.Model.Subscriptions
{
    public class Subscription : SubscriptionRequest
    {
        public Guid Id { get; set; }
        public string QueryName { get; set; }
        public string Destination { get; set; }
        public string SubscriptionId { get; set; }
        public SubscriptionControls Controls { get; set; }
        public IEnumerable<QueryParameter> Params { get; set; }
    }
}
