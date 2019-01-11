using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;

namespace FasTnT.Model.Subscriptions
{
    public class Subscription : SubscriptionRequest
    {
        public string QueryName { get; set; }
        public IList<QueryParameter> Params { get; set; }
        public string Destination { get; set; }
        public SubscriptionControls Controls { get; set; }
        public string SubscriptionId { get; set; }

        public IEnumerable<string> Select(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
