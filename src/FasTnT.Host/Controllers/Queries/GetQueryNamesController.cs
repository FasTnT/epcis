using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/GetQueryNames")]
    public class GetQueryNamesController : Controller
    {
        private readonly GetQueryNamesHandler _handler;

        public GetQueryNamesController(GetQueryNamesHandler handler) => _handler = handler;
        public async Task<GetQueryNamesResponse> Poll(GetQueryNames getQueryNamesRequest) => await _handler.Handle(getQueryNamesRequest);
    }
}
