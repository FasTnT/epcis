using FasTnT.Domain.Services;
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
        private readonly CaptureService _service;

        public EpcisCaptureController(CaptureService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Capture(Request request, CancellationToken cancellationToken)
        {
            switch (request)
            {
                case CaptureRequest captureRequest:
                    await _service.CaptureDocument(captureRequest, cancellationToken);
                    break;
                case EpcisQueryCallbackDocument queryCallbackDocument:
                    await _service.CaptureCallback(queryCallbackDocument, cancellationToken);
                    break;
                case EpcisQueryCallbackException queryCallbackException:
                    await _service.CaptureCallbackException(queryCallbackException, cancellationToken);
                    break;
            }

            return StatusCode(201);
        }
    }
}
