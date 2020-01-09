using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IDatabaseMigrator
    {
        Task Migrate();
        Task Rollback();
    }
}
