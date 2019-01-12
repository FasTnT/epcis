using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SubscribeHandler : ISubscriptionHandler<Subscription>
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public SubscribeHandler(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        public async Task<IEpcisResponse> Handle(Subscription subscribe)
        {
            // TODO: validate subscription request.
            await _subscriptionManager.Store(subscribe);

            return new SubscribeResponse();
        }
    }
}
