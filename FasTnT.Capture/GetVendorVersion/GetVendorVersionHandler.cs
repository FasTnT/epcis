using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using MediatR;

namespace FasTnT.Handlers.GetVendorVersion
{
    public class GetVendorVersionHandler : IRequestHandler<GetVendorVersionRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(GetVendorVersionRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new GetVendorVersionResponse { Version = Constants.VendorVersion });
        }
    }
}
