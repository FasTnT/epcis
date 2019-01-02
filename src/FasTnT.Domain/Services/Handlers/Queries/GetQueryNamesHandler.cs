using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class GetQueryNamesHandler : IQueryHandler<GetQueryNames>
    {
        public async Task<IEpcisResponse> Handle(GetQueryNames query)
        {
            return await Task.FromResult(new GetQueryNamesResponse
            {
                QueryNames = new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }
            });
        }
    }
}
