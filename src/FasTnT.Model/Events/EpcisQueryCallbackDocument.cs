using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisQueryCallbackDocument : Request
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IEnumerable<EpcisEvent> EventList { get; set; }
        public string SubscriptionName { get; set; }
    }
}
