using FasTnT.Domain.Model.Subscriptions;
using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionCreatedNotification : INotification
    {
        public Subscription Subscription { get; set; }
    }
}
