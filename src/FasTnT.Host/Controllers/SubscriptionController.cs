using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Subscription")]
    [Produces("application/xml")]
    public class SubscriptionController : Controller
    {
        private readonly SubscriptionService _service;

        public SubscriptionController(SubscriptionService service) => _service = service;

        [HttpPost("Subscribe", Name = "Subscribe to EPCIS repository")]
        public async Task Subscribe(Subscription request) => await _service.Process(request);

        [HttpPost("Unsubscribe", Name = "Unsubscribe from EPCIS repository")]
        public async Task Unsubscribe(UnsubscribeRequest request) => await _service.Process(request);
    }
}
