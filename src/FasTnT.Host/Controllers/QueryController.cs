using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
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
        {
            var parameters = default(QueryParameter);//ODataRequestParser.ParseFromQueryString(Request.QueryString);
            if (parameters == null)
            {
                return await _queryService.GetQueryNames(cancellationToken);
            }
            else
            {
                return await _queryService.Poll(new Poll { QueryName = QueryName }, cancellationToken);
            }
        }

        [HttpGet("{queryName}/events")]
        public async Task<object> Poll(string queryName, CancellationToken cancellationToken) 
            => await _queryService.Poll(new Poll { QueryName = queryName }, cancellationToken);

        [HttpGet("{queryName}/subscriptions")]
        public async Task ListSubscriptions(string queryName, CancellationToken cancellationToken) 
            => await _queryService.GetSubscriptionId(new GetSubscriptionIds { QueryName = queryName }, cancellationToken);

        [HttpDelete("{queryName}/subscriptions/{subscriptionId}")]
        public async Task Unsubscribe(string queryName, string subscriptionId, CancellationToken cancellationToken) 
            => await _queryService.Unsubscribe(new UnsubscribeRequest { SubscriptionId = subscriptionId }, cancellationToken);
    }
}