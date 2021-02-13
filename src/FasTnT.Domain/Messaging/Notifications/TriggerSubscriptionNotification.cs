using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class TriggerSubscriptionNotification : INotification
    {
        public string Name { get; set; }
    }
}
