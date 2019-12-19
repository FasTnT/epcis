using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Subscribe
{
    public class SubscribeHandler : IRequestHandler<SubscribeRequest, IEpcisResponse>
    {
        private readonly IMediator _mediator;

        public SubscribeHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEpcisResponse> Handle(SubscribeRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new SubscriptionCreatedNotification(), cancellationToken);

            return EmptyResponse.Value;
        }
    }
}
