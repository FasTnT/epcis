using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("api")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly QueryService _queryService;

        public QueryController(QueryService queryService) => _queryService = queryService;

        [HttpGet]
        [Route("queries")]
        public async Task<object> Queries() =>  await _queryService.Process(new GetQueryNames());

        // TODO: get query parameters from OData query string
        [HttpGet]
        [Route("queries/{queryName}/events")]
        public async Task<object> Poll(string queryName) => await _queryService.Process(new Poll { QueryName = queryName, Parameters = new QueryParameter[0] });

        [HttpGet]
        [Route("queries/{queryName}/subscriptions")]
        public async Task Unsubscribe(string queryName) => await _queryService.Process(new GetSubscriptionIds { QueryName = queryName });

        [HttpDelete]
        [Route("queries/{queryName}/subscriptions/{subscriptionId}")]
        public async Task Unsubscribe(string queryName, string subscriptionId) => _queryService.Process(new UnsubscribeRequest { SubscriptionId = subscriptionId });
    }
}