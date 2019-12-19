using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionRemovedNotification : INotification
    {
        public string SubscriptionId { get; set; }
    }
}
