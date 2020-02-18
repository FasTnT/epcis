using FasTnT.Domain.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions.Handlers
{
    public class SubscriptionRemovedHandler : INotificationHandler<SubscriptionRemovedNotification>
    {
        private readonly SubscriptionBackgroundService _service;

        public SubscriptionRemovedHandler(SubscriptionBackgroundService service)
        {
            _service = service;
        }

        public Task Handle(SubscriptionRemovedNotification notification, CancellationToken cancellationToken)
        {
            _service.Remove(notification.Subscription);

            return Task.CompletedTask;
        }
    }
}
