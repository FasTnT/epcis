using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class UnsubscribeHandler : ISubscriptionHandler<UnsubscribeRequest>
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public UnsubscribeHandler(ISubscriptionManager subscriptionManager, ISubscriptionBackgroundService backgroundService)
        {
            _subscriptionManager = subscriptionManager;
            _backgroundService = backgroundService;
        }

        public async Task<IEpcisResponse> Handle(UnsubscribeRequest query)
        {
            var subscriptions = await _subscriptionManager.GetAll();
            var subscription = subscriptions.SingleOrDefault(x => x.SubscriptionId == query.SubscriptionId);

            if(subscription == null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription with ID '{query.SubscriptionId}' does not exist.");
            }

            await _subscriptionManager.Delete(subscription.Id);
            _backgroundService.Remove(subscription);

            return new UnsubscribeResponse();
        }
    }
}
