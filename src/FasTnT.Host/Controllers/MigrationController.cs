using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Domain.Services.Setup;
using FasTnT.Host.Infrastructure.Attributes;

namespace FasTnT.Host.Controllers
{
    [DevelopmentOnly]
    [Route("Services/1.2")]
    public class MigrationController : Controller
    {
        private readonly IDatabaseMigrator _migrator;

        public MigrationController(IDatabaseMigrator migrator) => _migrator = migrator;

        [HttpPost(Name = "Create Database")]
        [Route("Migrate")]
        public async Task Migrate() => await _migrator.Migrate();

        [HttpPost(Name = "Drop Database")]
        [Route("Rollback")]
        public async Task Rollback() => await _migrator.Rollback();
    }
}
