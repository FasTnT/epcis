using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class PollRequest : IQueryRequest
    {
        public string QueryName { get; set; }
        public QueryParameter[] Parameters { get; set; }

        public class PollHandler : IRequestHandler<PollRequest, IEpcisResponse>
        {
            private readonly IEnumerable<IEpcisQuery> _queries;

            public PollHandler(IEnumerable<IEpcisQuery> queries)
            {
                _queries = queries;
            }

            public async Task<IEpcisResponse> Handle(PollRequest request, CancellationToken cancellationToken)
            {
                var query = _queries.FirstOrDefault(q => q.Name == request.QueryName)
                            ?? throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");
                
                return await query.Handle(request.Parameters, cancellationToken);
            }
        }
    }
}
