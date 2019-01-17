using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Model;
using FasTnT.Domain.Services.Handlers.Capture;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2/Capture/Events")]
    public class CaptureEventController : Controller
    {
        private readonly CaptureEventsHandler _handler;

        public CaptureEventController(CaptureEventsHandler handler) => _handler = handler;
        public async Task CaptureEvents(EpcisEventDocument events) => await _handler.Handle(events);
    }
}
