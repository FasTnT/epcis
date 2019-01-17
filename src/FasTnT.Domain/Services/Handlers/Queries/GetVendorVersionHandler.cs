using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class GetVendorVersionHandler
    {
        public Task<GetVendorVersionResponse> Handle(GetVendorVersion query) => Task.Run(() => new GetVendorVersionResponse { Version = "0.1.0" });
    }
}
