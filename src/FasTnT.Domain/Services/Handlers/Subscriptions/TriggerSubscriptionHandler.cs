using FasTnT.Domain.BackgroundTasks;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Subscriptions
{
    public class TriggerSubscriptionHandler
    {
        private readonly ISubscriptionBackgroundService _subscriptionService;

        public TriggerSubscriptionHandler(ISubscriptionBackgroundService subscriptionService) => _subscriptionService = subscriptionService;
        public Task Handle(TriggerSubscriptionRequest query) => Task.Run(() => _subscriptionService.Trigger(query.Trigger));
    }
}
