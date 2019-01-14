using FasTnT.Domain.Services.Dispatch;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2")]
    [Produces("application/xml")]
    public class SubscriptionsController : Controller
    {
        private readonly IDispatcher _dispatcher;

        public SubscriptionsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost(Name = "Subscribe to EPCIS server")]
        [Route("Subscription")]
        public async Task<IEpcisResponse> ManageSubscription(SubscriptionRequest request) => await _dispatcher.Dispatch(request);
    }
}
