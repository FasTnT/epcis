using FasTnT.Domain;
using FasTnT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v2_0
{
    [Route("v2_0/capture")]
    [JsonFormatter]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly CaptureDispatcher _dispatcher;

        public CaptureController(CaptureDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost]
        public async Task Post(Request request, CancellationToken cancellationToken) => await _dispatcher.DispatchDocument(request, cancellationToken);
    }
}