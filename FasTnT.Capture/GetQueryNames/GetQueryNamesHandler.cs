using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Handlers.GetQueryNames
{
    public class GetQueryNamesHandler : IRequestHandler<GetQueryNamesRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(GetQueryNamesRequest request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
