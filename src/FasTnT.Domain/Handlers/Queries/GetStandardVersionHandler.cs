using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Domain.Handlers.GetStandardVersion
{
    public class GetStandardVersionHandler : IRequestHandler<GetStandardVersionRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(GetStandardVersionRequest request, CancellationToken cancellationToken)
        {
            var result = new GetStandardVersionResponse { Version = Constants.StandardVersion };

            return await Task.FromResult(result);
        }
    }
}
