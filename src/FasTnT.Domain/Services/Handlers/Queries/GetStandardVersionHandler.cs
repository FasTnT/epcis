using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class GetStandardVersionHandler : IQueryHandler<GetStandardVersion>
    {
        public async Task<IEpcisResponse> Handle(GetStandardVersion query) => await Task.FromResult(new GetStandardVersionResponse { Version = "1.2" });
    }
}
