using FasTnT.Commands.Responses;
using FasTnT.Domain;
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
            ValidateRequest(request.Subscription);

            await _subscriptionManager.Store(request.Subscription, cancellationToken);
            await _mediator.Publish(new SubscriptionCreatedNotification { Subscription = request.Subscription }, cancellationToken);

            return EmptyResponse.Value;
        }

        private void ValidateRequest(Subscription subscription)
        {
            var query = _queries.FirstOrDefault(q => q.Name == subscription.QueryName);

            if (query == null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{subscription.QueryName}' is not implemented");
            }
            else if (!query.AllowSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query with name '{subscription.QueryName}' does not allow subscription");
            }
            else if (!(subscription.Schedule == null ^ string.IsNullOrEmpty(subscription.Trigger)))
            {
                throw new EpcisException(ExceptionType.SubscriptionControlsException, "Only one of the schedule and trigger must be provided");
            }
            else if (!QuerySchedule.IsValid(subscription.Schedule))
            {
                throw new EpcisException(ExceptionType.SubscriptionControlsException, "Provided schedule parameters are invalid");
            }

            EnsureDestinationIsValidURI(subscription);
            EnsureDestinationHasEndSlash(subscription);
        }

        private static void EnsureDestinationHasEndSlash(Subscription request) => request.Destination = $"{request.Destination.TrimEnd('/')}/";
        private static void EnsureDestinationIsValidURI(Subscription request) => UriValidator.Validate(request.Destination);
    }
}
