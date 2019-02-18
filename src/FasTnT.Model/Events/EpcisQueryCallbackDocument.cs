using System;

namespace FasTnT.Model
{
    public class EpcisQueryCallbackDocument : Request
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public EpcisEvent[] EventList { get; set; }
        public string SubscriptionName { get; set; }
    }
}
