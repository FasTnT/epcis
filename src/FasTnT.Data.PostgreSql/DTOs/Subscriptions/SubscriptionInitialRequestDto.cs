using FasTnT.Domain.Model.Subscriptions;
using System;

namespace FasTnT.Data.PostgreSql.DTOs.Subscriptions
{
    public class SubscriptionInitialRequestDto
    {
        public int SubscriptionId { get; set; }
        public DateTime? InitialRecordTime { get; set; }

        public static SubscriptionInitialRequestDto Create(Subscription subscription, int subscriptionId)
        {
            return new SubscriptionInitialRequestDto
            {
                SubscriptionId = subscriptionId,
                InitialRecordTime = subscription.InitialRecordTime
            };
        }
    }
}
