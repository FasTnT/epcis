using FasTnT.Domain.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Commands.Setup
{
    public class RollbackDatabaseRequest : IRequest
    {
        public class RollbackDatabaseHandler : IRequestHandler<RollbackDatabaseRequest>
        {
            private readonly IDatabaseMigrator _migrator;

            public RollbackDatabaseHandler(IDatabaseMigrator migrator)
            {
                _migrator = migrator;
            }

            public async Task<Unit> Handle(RollbackDatabaseRequest request, CancellationToken cancellationToken)
            {
                await _migrator.Rollback();

                return Unit.Value;
            }
        }
    }
}
