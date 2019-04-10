using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Setup
{
    public interface IDatabaseMigrator
    {
        Task Migrate(CancellationToken cancellationToken);
        Task Rollback(CancellationToken cancellationToken);
    }
}
