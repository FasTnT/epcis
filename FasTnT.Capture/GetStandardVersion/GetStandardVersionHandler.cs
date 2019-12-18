using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using MediatR;

namespace FasTnT.Handlers.GetStandardVersion
{
    public class GetStandardVersionHandler : IRequestHandler<GetStandardVersionRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(GetStandardVersionRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new GetStandardVersionResponse { Version = Constants.StandardVersion });
        }
    }
}
