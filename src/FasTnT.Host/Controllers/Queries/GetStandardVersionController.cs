using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/GetStandardVersion")]
    public class GetStandardVersionController : Controller
    {
        private readonly GetStandardVersionHandler _handler;

        public GetStandardVersionController(GetStandardVersionHandler handler) => _handler = handler;
        public async Task<GetStandardVersionResponse> Poll(GetStandardVersion getStandardVersionRequest) => await _handler.Handle(getStandardVersionRequest);
    }
}
