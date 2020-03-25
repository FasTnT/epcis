using FasTnT.Domain.Commands;
using FasTnT.Host.Infrastructure.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [Formatter(Format.Xml)]
    [ApiController, Route("v1_2/Capture")]
    public class EpcisCaptureController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EpcisCaptureController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task Capture(ICaptureRequest request, CancellationToken cancellationToken) => await _mediator.Send(request, cancellationToken);
    }
}
