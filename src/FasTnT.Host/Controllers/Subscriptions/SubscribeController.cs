using FasTnT.Domain.Services.Handlers.Subscriptions;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/Subscribe")]
    public class SubscribeController : Controller
    {
        private readonly SubscribeHandler _handler;

        public SubscribeController(SubscribeHandler handler) => _handler = handler;
        public async Task Poll(Subscription subscription) => await _handler.Handle(subscription);
    }
}
