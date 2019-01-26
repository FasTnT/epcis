using FasTnT.Domain.Services;
using FasTnT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/QueryCallback")]
    [Produces("application/xml")]
    public class QueryCallbackController : Controller
    {
        private readonly CallbackService _callbackService;

        public QueryCallbackController(CallbackService callbackService) => _callbackService = callbackService;

        [HttpPost("CallbackResults", Name = "Capture query callback results")]
        public async Task CallbackResults(EpcisQueryCallbackDocument callbackResult) => _callbackService.Process(callbackResult);

        [HttpPost("CallbackQueryTooLargeException", Name = "Capture query callback query too large exception")]
        public void CallbackQueryTooLargeException() { /* TODO: do something smart with this callback */ }

        [HttpPost("CallbackImplementationException", Name = "Capture query callback implementation exception")]
        public void CallbackImplementationException() { /* TODO: do something smart with this callback */ }
    }
}
