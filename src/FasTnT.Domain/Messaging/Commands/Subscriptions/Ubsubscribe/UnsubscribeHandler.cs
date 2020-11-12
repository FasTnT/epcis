using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Notifications;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class UnsubscribeHandler : IRequestHandler<UnsubscribeRequest, IEpcisResponse>
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IMediator _mediator;

        public UnsubscribeHandler(ISubscriptionManager subscriptionManager, IMediator mediator)
        {
            _subscriptionManager = subscriptionManager;
            _mediator = mediator;
        }

        public async Task<IEpcisResponse> Handle(UnsubscribeRequest request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionManager.GetById(request.SubscriptionId, cancellationToken);

            if (subscription == null)
            {
                throw new EpcisException(ExceptionType.NoSuchSubscriptionException, $"Subscription with ID '{request.SubscriptionId}' does not exist");
            }
            else
            {
                await _subscriptionManager.Delete(subscription.SubscriptionId, cancellationToken);
                await _mediator.Publish(new SubscriptionRemovedNotification { Subscription = subscription }, cancellationToken);
            }

            return EmptyResponse.Value;
        }
    }
}
