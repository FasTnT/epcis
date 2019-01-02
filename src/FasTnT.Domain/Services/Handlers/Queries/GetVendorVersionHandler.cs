using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class GetVendorVersionHandler : IQueryHandler<GetVendorVersion>
    {
        public async Task<IEpcisResponse> Handle(GetVendorVersion query) => await Task.FromResult(new GetVendorVersionResponse { Version = "1.0" });
    }
}
