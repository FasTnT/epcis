using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class GetSubscriptionIdsRequest : IQueryRequest
    {
        public string QueryName { get; set; }

        public class GetSubscriptionIdsHandler : IRequestHandler<GetSubscriptionIdsRequest, IEpcisResponse>
        {
            private readonly ISubscriptionManager _subscriptionManager;

            public GetSubscriptionIdsHandler(ISubscriptionManager subscriptionManager)
            {
                _subscriptionManager = subscriptionManager;
            }

            public async Task<IEpcisResponse> Handle(GetSubscriptionIdsRequest request, CancellationToken cancellationToken)
            {
                var subscriptionIds = await _subscriptionManager.GetSubscriptionIds(cancellationToken);

                return new GetSubscriptionIdsResponse { SubscriptionIds = subscriptionIds };
            }
        }
    }
}
