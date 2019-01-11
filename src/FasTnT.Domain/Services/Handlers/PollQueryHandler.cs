using FasTnT.Model.Responses;
using System;
using System.Threading.Tasks;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using System.Linq;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Model.Exceptions;

namespace FasTnT.Domain.Services.Handlers
{
    public class PollQueryHandler : IQueryHandler<Poll>
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IEventRepository _eventRepository;

        public PollQueryHandler(IEpcisQuery[] queries, IEventRepository eventRepository)
        {
            _queries = queries;
            _eventRepository = eventRepository;
        }

        public async Task<IEpcisResponse> Handle(Poll query)
        {
            var knownHandler = _queries.SingleOrDefault(x => x.Name == query.QueryName);

            if (knownHandler == null)
            {
                throw new Exception($"Unknown query: '{query.QueryName}'");
            }
            else
            {
                try
                {
                    knownHandler.ValidateParameters(query.Parameters);

                    var results = await knownHandler.Execute(query.Parameters, _eventRepository);
                    return new PollResponse { QueryName = query.QueryName, Entities = results };
                }
                catch(Exception ex)
                {
                    throw new EpcisException(ExceptionType.QueryParameterException, ex.Message);
                }
            }
        }
    }
}
