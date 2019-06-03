using FasTnT.Domain;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [SoapFormatter]
    [Route("v1_2/Query")]
    public class EpcisQueryService : Controller
    {
        private readonly QueryDispatcher _dispatcher;

        public EpcisQueryService(QueryDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost]
        public async Task<IEpcisResponse> Query(EpcisQuery query, CancellationToken cancellationToken) 
            => await _dispatcher.DispatchQuery(query, cancellationToken);
    }
}
