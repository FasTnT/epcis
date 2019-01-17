using FasTnT.Domain.Services.Handlers.Subscriptions;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Subscription")]
    public class TriggerSubscriptionController : Controller
    {
        private readonly TriggerSubscriptionHandler _handler;

        public TriggerSubscriptionController(TriggerSubscriptionHandler handler) => _handler = handler;

        [HttpGet(Name = "Trigger a subscription 'trigger' schedule")]
        [Route("Trigger/{name}")]
        public async Task TriggerSubscription(string name) => await _handler.Handle(new TriggerSubscriptionRequest { Trigger = name });
    }
}
