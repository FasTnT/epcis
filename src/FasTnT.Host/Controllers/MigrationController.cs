using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FasTnT.Domain.Services.Setup;
using FasTnT.Host.Infrastructure.Attributes;
using FasTnT.Domain.Persistence;
using System;
using Microsoft.AspNetCore.Authorization;

namespace FasTnT.Host.Controllers
{
    [Authorize]
    [DevelopmentOnly]
    [Route("Services/1.2")]
    public class MigrationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MigrationController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpPost(Name = "Create Database")]
        [Route("Migrate")]
        public async Task Migrate() => await Commit(manager => manager.Migrate());

        [HttpPost(Name = "Drop Database")]
        [Route("Rollback")]
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
