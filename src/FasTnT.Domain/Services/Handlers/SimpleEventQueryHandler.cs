using FasTnT.Model.Queries.Implementations.PredefinedQueries;
using FasTnT.Model.Responses;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SimpleEventQueryHandler : IQueryHandler<SimpleEventQuery>
    {
        private readonly ISimpleEventQueryExecutor _visitor;

        public SimpleEventQueryHandler(ISimpleEventQueryExecutor visitor) => _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));

        public async Task<IEpcisResponse> Handle(SimpleEventQuery query)
        {
            var results = await _visitor.Execute(query);

            return new PollResponse { QueryName = SimpleEventQuery.Name, Entities = results };
        }
    }
}
