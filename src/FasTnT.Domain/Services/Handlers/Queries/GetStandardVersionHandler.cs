using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class GetStandardVersionHandler
    {
        public Task<GetStandardVersionResponse> Handle(GetStandardVersion query) => Task.Run(() => new GetStandardVersionResponse { Version = "1.2" });
    }
}
