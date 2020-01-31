using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class SubscribeRequest : IQueryRequest
    {
        public Subscription Subscription { get; set; }

        public class SubscribeHandler : IRequestHandler<SubscribeRequest, IEpcisResponse>
        {
            private readonly IEnumerable<IEpcisQuery> _queries;
            private readonly ISubscriptionManager _subscriptionManager;
            private readonly IMediator _mediator;

            public SubscribeHandler(IEnumerable<IEpcisQuery> queries, ISubscriptionManager subscriptionManager, IMediator mediator)
            {
                _queries = queries;
                _subscriptionManager = subscriptionManager;
                _mediator = mediator;
            }

            public async Task<IEpcisResponse> Handle(SubscribeRequest request, CancellationToken cancellationToken)
            {
                var query = _queries.SingleOrDefault(q => q.Name == request.Subscription.QueryName);

                if (query == null)
                {
                    throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.Subscription.QueryName}' is not implemented");
                }
                else if (!query.AllowSubscription)
                {
                    throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query with name '{request.Subscription.QueryName}' does not allow subscription");
                }
                else
                {
                    await _subscriptionManager.Store(request.Subscription, cancellationToken);
                    await _mediator.Publish(new SubscriptionCreatedNotification { Subscription = request.Subscription }, cancellationToken);

                    return EmptyResponse.Value;
                }
            }
        }
    }
}
