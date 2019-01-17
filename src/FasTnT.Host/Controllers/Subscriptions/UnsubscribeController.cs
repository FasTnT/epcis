using FasTnT.Domain.Services.Handlers.Subscriptions;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Query/Unsubscribe")]
    public class UnsubscribeController : Controller
    {
        private readonly UnsubscribeHandler _handler;

        public UnsubscribeController(UnsubscribeHandler handler) => _handler = handler;
        public async Task Poll(UnsubscribeRequest unsubscribeRequest) => await _handler.Handle(unsubscribeRequest);
    }
}
