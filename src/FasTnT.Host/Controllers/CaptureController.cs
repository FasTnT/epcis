using FasTnT.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("capture")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly CaptureService _captureService;

        public CaptureController(CaptureService captureService) => _captureService = captureService;

        // TODO: parse request body
        [HttpPost]
        public async Task Post(CancellationToken cancellationToken) 
            => await _captureService.Capture(null, cancellationToken);
    }
}