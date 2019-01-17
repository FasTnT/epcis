using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/Poll")]
    public class PollQueryController : Controller
    {
        private readonly PollQueryHandler _handler;

        public PollQueryController(PollQueryHandler handler) => _handler = handler;
        public async Task<PollResponse> Poll(Poll pollRequest) => await _handler.Handle(pollRequest);
    }
}
