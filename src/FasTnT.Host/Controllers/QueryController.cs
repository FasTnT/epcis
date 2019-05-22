using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [ApiVersion("1y0", Deprecated = true)]
    [ApiVersion("1.2")]
    [ApiVersion("2.0")]
    [Route("{v:apiVersion}/queries")]
    [ApiController]
    public class QueryController : Controller
    {
        private const string QueryName = "SimpleEventQuery";
        private readonly QueryService _queryService;

        public QueryController(QueryService queryService) => _queryService = queryService;

        [HttpGet("")]
        public async Task<object> Queries(CancellationToken cancellationToken)
            => await _queryService.GetQueryNames(cancellationToken);

        [HttpGet("{queryName}/events")]
        public async Task<object> Poll(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken) 
            => await _queryService.Poll(new Poll { QueryName = queryName, Parameters = parameters }, cancellationToken);

        [HttpGet("{queryName}/subscriptions")]
        public async Task ListSubscriptions(string queryName, CancellationToken cancellationToken) 
            => await _queryService.GetSubscriptionId(new GetSubscriptionIds { QueryName = queryName }, cancellationToken);

        [HttpDelete("{queryName}/subscriptions/{subscriptionId}")]
        public async Task Unsubscribe(string subscriptionId, CancellationToken cancellationToken) 
            => await _queryService.Unsubscribe(new UnsubscribeRequest { SubscriptionId = subscriptionId }, cancellationToken);
    }
}