using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Subscribe
{
    public class SubscribeHandler : IRequestHandler<SubscribeRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(SubscribeRequest request, CancellationToken cancellationToken)
        {
            return EmptyResponse.Default;
        }
    }
}
