using System;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Persistence.Dapper
{
    public class SubscriptionEntity : Subscription
    {
        public Guid Id { get; set; }
    }
}
