using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Setup
{
    public interface IDatabaseMigrator
    {
        Task Migrate();
    }
}
