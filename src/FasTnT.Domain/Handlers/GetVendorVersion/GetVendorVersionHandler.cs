using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Domain.Handlers.GetVendorVersion
{
    public class GetVendorVersionHandler : IRequestHandler<GetVendorVersionRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(GetVendorVersionRequest request, CancellationToken cancellationToken)
        {
            var result = new GetVendorVersionResponse { Version = Constants.VendorVersion };

            return await Task.FromResult(result);
        }
    }
}
