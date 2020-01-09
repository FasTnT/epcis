using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public class SimpleEventQuery : IEpcisQuery
    {
        private readonly IEventFetcher _eventFetcher;

        public SimpleEventQuery(IEventFetcher eventFetcher)
        {
            _eventFetcher = eventFetcher;
        }

        public string Name => "SimpleEventQuery";

        public async Task<IEpcisResponse> Handle(QueryParameter[] parameters, CancellationToken cancellationToken)
        {
            foreach(var parameter in parameters)
            {
                // TODO: apply correct parameter.
                _eventFetcher.Apply(new SimpleParameterFilter { });
            }

            var result = await _eventFetcher.Fetch(cancellationToken);

            return new PollResponse
            {
                QueryName = Name,
                EventList = result
            };
        }
    }
}
