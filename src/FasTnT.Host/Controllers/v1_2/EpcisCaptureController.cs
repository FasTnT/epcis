using FasTnT.Domain;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [XmlFormatter]
    [Route("v1_2/Capture")]
    public class EpcisCaptureService : Controller
    {
        private readonly CaptureDispatcher _dispatcher;

        public EpcisCaptureService(CaptureDispatcher service) => _dispatcher = service;

        [HttpPost]
        public async Task Capture(Request request, CancellationToken cancellationToken)
            => await _dispatcher.DispatchDocument(request, cancellationToken);
    }
}
