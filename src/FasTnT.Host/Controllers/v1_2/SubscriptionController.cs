using FasTnT.Domain.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [Route("v1_2/Subscription/Trigger")]
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{triggerName}")]
        public async Task TriggerSubscription(string triggerName, CancellationToken cancellationToken) => await _mediator.Publish(new TriggerSubscriptionRequest { Name = triggerName }, cancellationToken);
    }
}
