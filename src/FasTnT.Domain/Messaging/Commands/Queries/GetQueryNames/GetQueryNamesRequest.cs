using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Queries;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class GetQueryNamesRequest : IQueryRequest
    {
        public class GetQueryNamesHandler : IRequestHandler<GetQueryNamesRequest, IEpcisResponse>
        {
            private readonly IEnumerable<IEpcisQuery> _queries;

            public GetQueryNamesHandler(IEnumerable<IEpcisQuery> queries)
            {
                _queries = queries;
            }

            public async Task<IEpcisResponse> Handle(GetQueryNamesRequest request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new GetQueryNamesResponse
                {
                    QueryNames = _queries.Select(q => q.Name).ToArray()
                });
            }
        }
    }
}
