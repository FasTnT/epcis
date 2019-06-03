using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v2_0
{
    [Route("v2_0/events")]
    [JsonFormatter]
    [ApiController]
    public class EventsController : Controller
    {
        private const string QueryName = "SimpleEventQuery";
        private readonly QueryService _queryService;

        public EventsController(QueryService queryService) => _queryService = queryService;

        [HttpGet]
        public async Task<IEnumerable<string>> ListEventTypes(CancellationToken cancellationToken) 
            => (await _queryService.GetEventTypes(cancellationToken)).EventTypes;

        [HttpGet("all")]
        public async Task<object> ListAllEventTypes(CancellationToken cancellationToken) 
            => await _queryService.Poll(new Poll { QueryName = QueryName }, cancellationToken);

        [HttpGet("{eventType}")]
        public async Task<object> ListEventsOfType(string eventType, QueryParameter[] parameters, CancellationToken cancellationToken)
        {
            parameters = Enumerable.Append(parameters, new QueryParameter{ Name = "eventType", Values = new []{ eventType } }).ToArray();

            return await _queryService.Poll(new Poll { QueryName = QueryName, Parameters = parameters }, cancellationToken);
        }

        [HttpGet("{eventType}/{eventId}")]
        public async Task<object> GetEventById(string eventType, string eventId, QueryParameter[] parameters, CancellationToken cancellationToken)
        {
            parameters = Enumerable.Concat(parameters, new[] 
            {
                new QueryParameter{ Name = "eventType", Values = new []{ eventType } },
                new QueryParameter{ Name = "EQ_eventID", Values = new []{ eventId } }
            }).ToArray();

            return await _queryService.Poll(new Poll { QueryName = QueryName, Parameters = parameters }, cancellationToken);
        }
    }
}