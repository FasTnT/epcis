using FasTnT.Domain;
using FasTnT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Authorize]
    [Route("EpcisServices/1.2/Capture")]
    [Consumes("application/xml", "text/xml", "application/json")]
    public class EpcisCaptureController : Controller
    {
        private readonly CaptureDispatcher _dispatcher;

        public EpcisCaptureController(CaptureDispatcher service) => _dispatcher = service;

        [HttpPost]
        public async Task Capture(Request request, CancellationToken cancellationToken)
            => await _dispatcher.DispatchDocument(request, cancellationToken);
    }
}
