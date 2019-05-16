using FasTnT.Domain.Services;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries;
using FasTnT.Model.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.2")]
    [ApiVersion("2.0")]
    [Route("{v:apiVersion}/events")]
    [ApiController]
    public class EventsController : Controller
    {
        private const string QueryName = "SimpleEventQuery";
        private readonly QueryService _queryService;

        public EventsController(QueryService queryService) => _queryService = queryService;

        [HttpGet("")]
        public string[] ListEventTypes() => Enumeration.GetAll<EventType>().Select(x => x.DisplayName).ToArray();

        [HttpGet("all")]
        public async Task<object> ListAllEventTypes(CancellationToken cancellationToken) 
            => await _queryService.Poll(new Poll { QueryName = QueryName }, cancellationToken);

        [HttpGet("{eventType}")]
        public async Task<object> ListEventsOfType(string eventType, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                new QueryParameter{ Name = "eventType", Values = new []{ eventType } }
            };

            return await _queryService.Poll(new Poll { QueryName = QueryName, Parameters = parameters }, cancellationToken);
        }

        [HttpGet("{eventType}/{eventId}")]
        public async Task<object> GetEventById(string eventType, string eventId, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                new QueryParameter{ Name = "eventType", Values = new []{ eventType } },
                new QueryParameter{ Name = "EQ_eventID", Values = new []{ eventId } }
            };

            return await _queryService.Poll(new Poll { QueryName = QueryName, Parameters = parameters }, cancellationToken);
        }
    }
}