using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Domain.Services.Setup;
using FasTnT.Host.Infrastructure.Attributes;
using FasTnT.Domain.Persistence;
using System;

namespace FasTnT.Host.Controllers
{
    [DevelopmentOnly]
    [Route("Services/1.2/Database")]
    public class MigrationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MigrationController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpPost("Migrate", Name = "Create Database")]
        public async Task Migrate() => await Commit(manager => manager.Migrate());

        [HttpPost("Rollback", Name = "Drop Database")]
        public async Task Rollback() => await Commit(manager => manager.Rollback());

        private async Task Commit(Func<IDatabaseMigrator, Task> action)
        {
            using (new CommitOnDispose(_unitOfWork))
            {
                await action(_unitOfWork.DatabaseManager);
            }
        }
    }
}
