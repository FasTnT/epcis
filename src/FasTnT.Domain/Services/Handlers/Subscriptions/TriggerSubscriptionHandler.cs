using FasTnT.Domain.BackgroundTasks;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Subscriptions
{
    public class TriggerSubscriptionHandler : ISubscriptionHandler<TriggerSubscriptionRequest>
    {
        private readonly ISubscriptionBackgroundService _subscriptionService;

        public TriggerSubscriptionHandler(ISubscriptionBackgroundService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public Task<IEpcisResponse> Handle(TriggerSubscriptionRequest query)
        {
            _subscriptionService.Trigger(query.Trigger);

            return Task.FromResult(default(IEpcisResponse));
        }
    }
}
