using FasTnT.Domain.Services.Setup;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper.Setup
{
    public class PgSqlDatabaseMigrator : IDatabaseMigrator
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlDatabaseMigrator(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            using (var msi = new MemoryStream(Convert.FromBase64String(SqlRequests.CreateDatabaseZipped)))
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            using (var sr = new StreamReader(gs))
            {
                await _unitOfWork.Execute(await sr.ReadToEndAsync(), null, cancellationToken);
            }
        }

        public async Task Rollback(CancellationToken cancellationToken)
        {
            using (var msi = new MemoryStream(Convert.FromBase64String(SqlRequests.DropDatabaseZipped)))
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            using (var sr = new StreamReader(gs))
            {
                await _unitOfWork.Execute(await sr.ReadToEndAsync(), null, cancellationToken);
            }
        }
    }
}
