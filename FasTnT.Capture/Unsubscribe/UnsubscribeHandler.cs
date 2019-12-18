using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Unsubscribe
{
    public class UnsubscribeHandler : IRequestHandler<UnsubscribeRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(UnsubscribeRequest request, CancellationToken cancellationToken)
        {
            return EmptyResponse.Default;
        }
    }
}
