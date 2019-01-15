using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Domain.Services.Dispatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Model;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2")]
    [Produces("application/xml")]
    public class EpcisController : Controller
    {
        private readonly IDispatcher _dispatcher;

        public EpcisController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost(Name = "Capture Endpoint")]
        [Route("Capture")]
        public async Task Capture([FromBody] Request document) => await _dispatcher.Dispatch(document);

        [HttpPost(Name = "Query Endpoint")]
        [Route("Query")]
        public async Task<IEpcisResponse> Query([FromBody] EpcisQuery query) => await _dispatcher.Dispatch(query);
    }
}
