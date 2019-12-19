using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Unsubscribe
{
    public class UnsubscribeHandler : IRequestHandler<UnsubscribeRequest, IEpcisResponse>
    {
        private readonly IMediator _mediator;

        public UnsubscribeHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEpcisResponse> Handle(UnsubscribeRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new SubscriptionRemovedNotification(), cancellationToken);

            return EmptyResponse.Value;
        }
    }
}
