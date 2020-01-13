using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.Subscribe
{
    public class SubscribeHandler : IRequestHandler<SubscribeRequest, IEpcisResponse>
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IMediator _mediator;

        public SubscribeHandler(IEpcisQuery[] queries, IMediator mediator)
        {
            _queries = queries;
            _mediator = mediator;
        }

        public async Task<IEpcisResponse> Handle(SubscribeRequest request, CancellationToken cancellationToken)
        {
            var query = _queries.SingleOrDefault(q => q.Name == request.QueryName);

            if (query == null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");
            }
            else if (!query.AllowSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query with name '{request.QueryName}' does not allow subscription");
            }
            else
            {
                // TODO: validate parameters and store subscription in Database
                await _mediator.Publish(new SubscriptionCreatedNotification(), cancellationToken);

                return EmptyResponse.Value;
            }
        }
    }
}
