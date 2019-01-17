using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class GetQueryNamesHandler
    {
        private readonly IEpcisQuery[] _queries;

        public GetQueryNamesHandler(IEpcisQuery[] queries) => _queries = queries;

        public Task<GetQueryNamesResponse> Handle(GetQueryNames query) => Task.Run(() => new GetQueryNamesResponse { QueryNames = _queries.Select(x => x.Name) });
    }
}
