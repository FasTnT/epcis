using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Domain.Services.Setup;

namespace FasTnT.Host.Controllers
{
    [Route("Services/1.2")]
    public class MigrationController : Controller
    {
        private readonly IDatabaseMigrator _migrator;

        public MigrationController(IDatabaseMigrator migrator) => _migrator = migrator;

        [HttpPost(Name = "Create Database")]
        [Route("Migrate")]
        public async Task Migrate() => await _migrator.Migrate();
    }
}
