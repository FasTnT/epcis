using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/GetVendorVersion")]
    public class GetVendorVersionController : Controller
    {
        private readonly GetVendorVersionHandler _handler;

        public GetVendorVersionController(GetVendorVersionHandler handler) => _handler = handler;
        public async Task<GetVendorVersionResponse> Poll(GetVendorVersion getVendorVersionRequest) => await _handler.Handle(getVendorVersionRequest);
    }
}
