using FasTnT.Model.Responses;
using System;
using System.Threading.Tasks;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using System.Linq;
using FasTnT.Model.Exceptions;
using FasTnT.Domain.Persistence;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class PollQueryHandler : IQueryHandler<Poll>
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IUnitOfWork _unitOfWork;

        public PollQueryHandler(IEpcisQuery[] queries, IUnitOfWork unitOfWork)
        {
            _queries = queries;
            _unitOfWork = unitOfWork;
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

                    var results = await knownHandler.Execute(query.Parameters, _unitOfWork);
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
