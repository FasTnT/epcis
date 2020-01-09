using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Domain.Handlers.GetQueryNames
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
