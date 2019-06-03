using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Controllers
{
    [Route("Setup/Database")]
    public class EpcisMigrationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EpcisMigrationController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpPost("Migrate")]
        public async Task Migrate(CancellationToken cancellationToken) 
            => await _unitOfWork.Execute(u => u.DatabaseManager.Migrate(cancellationToken));

        [HttpPost("Rollback")]
        public async Task Rollback(CancellationToken cancellationToken) 
            => await _unitOfWork.Execute(u => u.DatabaseManager.Rollback(cancellationToken));
    }
}
