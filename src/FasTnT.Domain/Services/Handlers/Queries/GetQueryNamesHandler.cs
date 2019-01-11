using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class GetQueryNamesHandler : IQueryHandler<GetQueryNames>
    {
        private readonly IEpcisQuery[] _queries;

        public GetQueryNamesHandler(IEpcisQuery[] queries) => _queries = queries;

        public async Task<IEpcisResponse> Handle(GetQueryNames query) => await Task.FromResult(new GetQueryNamesResponse { QueryNames = _queries.Select(x => x.Name) });
    }
}
