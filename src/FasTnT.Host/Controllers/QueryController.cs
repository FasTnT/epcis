using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Authorize]
    [Route("Services/1.2/Query")]
    [Produces("application/xml")]
    public class QueryController : Controller
    {
        private readonly QueryService _service;

        public QueryController(QueryService service) => _service = service;

        [HttpPost("GetQueryNames", Name = "List all available query names")]
        public async Task<GetQueryNamesResponse> GetQueryNames(GetQueryNames request) => await _service.Process(request);

        [HttpPost("GetStandardVersion", Name = "Get implemented EPCIS standard version")]
        public async Task<GetStandardVersionResponse> GetStandardVersion(GetStandardVersion request) => await _service.Process(request);

        [HttpPost("GetVendorVersion", Name = "Get product version")]
        public async Task<GetVendorVersionResponse> GetVendorVersion(GetVendorVersion request) => await _service.Process(request);

        [HttpPost("Poll", Name = "Perform a poll query")]
        public async Task<PollResponse> Poll(Poll request) => await _service.Process(request);

        [HttpPost("GetSubscriptionIds", Name = "List all subscription IDs for a query")]
        public async Task<GetSubscriptionIdsResult> GetSubscriptionIds(GetSubscriptionIds request) => await _service.Process(request);
    }
}
