using FasTnT.Commands.Responses;
using FasTnT.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
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
