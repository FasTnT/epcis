using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class GetSubscriptionIdsHandler : IQueryHandler<GetSubscriptionIds>
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public GetSubscriptionIdsHandler(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        public async Task<IEpcisResponse> Handle(GetSubscriptionIds query)
        {
            var subscriptions = (await _subscriptionManager.GetAll()).Where(s => s.QueryName == query.QueryName);

            return new GetSubscriptionIdsResult
            {
                SubscriptionIds = subscriptions.Select(x => x.SubscriptionId).ToArray()
            };
        }
    }
}
