using FasTnT.Domain.Services;
using FasTnT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.2")]
    [ApiVersion("2.0")]
    [Route("v{v:apiVersion}/capture")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly CaptureService _captureService;

        public CaptureController(CaptureService captureService) => _captureService = captureService;

        [HttpPost]
        public async Task Post(CaptureRequest request, CancellationToken cancellationToken) => await _captureService.CaptureDocument(request, cancellationToken);
    }
}