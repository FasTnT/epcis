using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.GetSubscriptionIds
{
    public class GetSubscriptionIdsHandler : IRequestHandler<GetSubscriptionIdsRequest, IEpcisResponse>
    {
        public Task<IEpcisResponse> Handle(GetSubscriptionIdsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
