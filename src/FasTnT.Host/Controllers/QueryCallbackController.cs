using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
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
        public async Task CallbackResults(EpcisQueryCallbackDocument callbackResult) => await _callbackService.Process(callbackResult);

        [HttpPost("CallbackQueryTooLargeException", Name = "Capture query callback query too large exception")]
        public async Task CallbackQueryTooLargeException(EpcisQueryCallbackException callbackResult) => await _callbackService.ProcessException(callbackResult);

        [HttpPost("CallbackImplementationException", Name = "Capture query callback implementation exception")]
        public async Task CallbackImplementationException(EpcisQueryCallbackException callbackResult) => await _callbackService.ProcessException(callbackResult);
    }
}
