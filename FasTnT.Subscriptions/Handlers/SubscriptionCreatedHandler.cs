using FasTnT.Domain.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions.Handlers
{
    public class SubscriptionCreatedHandler : INotificationHandler<SubscriptionCreatedNotification>
    {
        private readonly SubscriptionBackgroundService _service;

        public SubscriptionCreatedHandler(SubscriptionBackgroundService service)
        {
            _service = service;
        }

        public Task Handle(SubscriptionCreatedNotification notification, CancellationToken cancellationToken)
        {
            _service.Register(notification.Subscription);

            return Task.CompletedTask;
        }
    }
}
