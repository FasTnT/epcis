using FasTnT.Domain.Services;
using FasTnT.Host.Infrastructure.Attributes;
using FasTnT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Authorize]
    [Route("Services/1.2/Capture")]
    [Produces("application/xml")]
    public class CaptureController : Controller
    {
        private CaptureService _service;

        public CaptureController(CaptureService service) => _service = service;

        [Limit]
        [HttpPost("Events", Name = "Capture EPCIS Events")]
        public async Task Capture(EpcisEventDocument eventDocument) => await _service.Capture(eventDocument);

        [Limit]
        [HttpPost("MasterData", Name = "Capture CBV master data document")]
        public async Task Capture(EpcisMasterdataDocument masterDataDocument) => await _service.Capture(masterDataDocument);
    }
}
