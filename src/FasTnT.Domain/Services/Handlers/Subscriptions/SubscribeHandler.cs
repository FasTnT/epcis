using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SubscribeHandler : ISubscriptionHandler<Subscription>
    {
        private readonly IEpcisQuery[] _queries;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public SubscribeHandler(IEpcisQuery[] queries, ISubscriptionManager subscriptionManager, ISubscriptionBackgroundService backgroundService)
        {
            _queries = queries;
            _subscriptionManager = subscriptionManager;
            _backgroundService = backgroundService;
        }

        public async Task<IEpcisResponse> Handle(Subscription request)
        {
            EnsureQueryAllowsSubscription(request);
            await EnsureSubscriptionDoesNotExist(request);

            await _subscriptionManager.Store(request);
            _backgroundService.Register(request);

            return new SubscribeResponse();
        }

        private async Task EnsureSubscriptionDoesNotExist(Subscription request)
        {
            var subscription = await _subscriptionManager.GetById(request.SubscriptionId);
            if (subscription != null)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Subscription '{request.QueryName}' already exist.");
            }
        }

        private void EnsureQueryAllowsSubscription(Subscription subscribe)
        {
            var query = _queries.SingleOrDefault(x => x.Name == subscribe.QueryName);

            if(query == null || !query.AllowSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscribe.QueryName}' does not exist or doesn't allow subscription");
            }
        }
    }
}
