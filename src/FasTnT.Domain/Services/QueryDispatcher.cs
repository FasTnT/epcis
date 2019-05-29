using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class QueryDispatcher
    {
        private readonly QueryService _service;

        public QueryDispatcher(QueryService service) => _service = service;

        public async Task<IEpcisResponse> DispatchQuery(EpcisQuery query, CancellationToken cancellationToken)
        {
            var response = default(IEpcisResponse);

            switch (query)
            {
                case GetQueryNames _:
                    response = await _service.GetQueryNames(cancellationToken); break;
                case GetStandardVersion _:
                    response = await _service.GetStandardVersion(cancellationToken); break;
                case GetVendorVersion _:
                    response = await _service.GetVendorVersion(cancellationToken); break;
                case GetSubscriptionIds getSubscriptionIds:
                    response = await _service.GetSubscriptionId(getSubscriptionIds, cancellationToken); break;
                case Poll poll:
                    response = await _service.Poll(poll, cancellationToken); break;
                case Subscription subscription:
                    await _service.Subscribe(subscription, cancellationToken); break;
                case UnsubscribeRequest unsubscribeRequest:
                    await _service.Unsubscribe(unsubscribeRequest, cancellationToken); break;
            }

            return response;
        }
    }
}
