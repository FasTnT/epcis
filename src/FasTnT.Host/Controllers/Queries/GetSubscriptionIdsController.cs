using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/GetSubscriptionIDs")]
    public class GetSubscriptionIdsController : Controller
    {
        private readonly GetSubscriptionIdsHandler _handler;

        public GetSubscriptionIdsController(GetSubscriptionIdsHandler handler) => _handler = handler;
        public async Task<GetSubscriptionIdsResult> Poll(GetSubscriptionIds getStandardVersionRequest) => await _handler.Handle(getStandardVersionRequest);
    }
}
