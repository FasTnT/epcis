using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionCreatedNotification : INotification
    {
        public string SubscriptionId { get; set; }
    }
}
