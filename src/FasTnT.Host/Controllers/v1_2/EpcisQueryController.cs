using FasTnT.Domain;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    public abstract class EpcisQueryController : Controller
    {
        private readonly QueryDispatcher _dispatcher;

        public EpcisQueryController(QueryDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost]
        public async Task<IEpcisResponse> Query(EpcisQuery query, CancellationToken cancellationToken)
            => await _dispatcher.DispatchQuery(query, cancellationToken);
    }
}
