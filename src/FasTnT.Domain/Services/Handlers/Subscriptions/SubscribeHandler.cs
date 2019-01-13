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

        public SubscribeHandler(IEpcisQuery[] queries, ISubscriptionManager subscriptionManager)
        {
            _queries = queries;
            _subscriptionManager = subscriptionManager;
        }

        public async Task<IEpcisResponse> Handle(Subscription subscribe)
        {
            EnsureQueryAllowsSubscription(subscribe);
            // TODO: validate subscription request.
            await _subscriptionManager.Store(subscribe);

            return new SubscribeResponse();
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
