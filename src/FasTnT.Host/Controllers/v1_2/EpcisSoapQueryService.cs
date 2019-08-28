using FasTnT.Domain;
using FasTnT.Host.Infrastructure.Attributes;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [Formatter(Format.Soap)]
    [Route("v1_2/Query.svc")]
    public class EpcisSoapQueryService : Controller
    {
        private readonly QueryDispatcher _dispatcher;

        public EpcisSoapQueryService(QueryDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost]
        public async Task<IEpcisResponse> Query(EpcisQuery query, CancellationToken cancellationToken) 
            => await _dispatcher.DispatchQuery(query, cancellationToken);
    }
}
