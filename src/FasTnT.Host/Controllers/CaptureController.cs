using FasTnT.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly CaptureService _captureService;

        public CaptureController(CaptureService captureService) => _captureService = captureService;

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            return await Task.FromResult(new OkResult());
        }
    }
}