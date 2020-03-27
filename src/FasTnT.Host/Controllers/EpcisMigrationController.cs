using FasTnT.Domain.Commands.Setup;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [ApiController, Route("Setup/Database")]
    public class EpcisMigrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EpcisMigrationController(IMediator mediator) => _mediator = mediator;

        [HttpPost("Migrate")]
        public async Task Migrate(CancellationToken cancellationToken)
            => await _mediator.Send(new MigrateDatabaseRequest(), cancellationToken);

        [HttpPost("Rollback")]
        public async Task Rollback(CancellationToken cancellationToken)
            => await _mediator.Send(new RollbackDatabaseRequest(), cancellationToken);
    }
}
