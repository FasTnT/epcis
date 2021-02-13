using FasTnT.Domain.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions.Handlers
{
    public class SubscriptionTriggerHandler : INotificationHandler<TriggerSubscriptionNotification>
    {
        private readonly SubscriptionBackgroundService _service;

        public SubscriptionTriggerHandler(SubscriptionBackgroundService service)
        {
            _service = service;
        }

        public Task Handle(TriggerSubscriptionNotification notification, CancellationToken cancellationToken)
        {
            _service.Trigger(notification.Name);

            return Task.CompletedTask;
        }
    }
}
