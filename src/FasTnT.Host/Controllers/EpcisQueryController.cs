using FasTnT.Domain;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Authorize]
    [Route("EpcisServices/1.2/Query")]
    [Consumes("application/xml", "text/xml")]
    [Produces("application/xml", "text/xml")]
    public class EpcisQueryController : Controller
    {
        private readonly QueryDispatcher _dispatcher;

        public EpcisQueryController(QueryDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost]
        public async Task<IEpcisResponse> Query(EpcisQuery query, CancellationToken cancellationToken) 
            => await _dispatcher.DispatchQuery(query, cancellationToken);
    }
}
