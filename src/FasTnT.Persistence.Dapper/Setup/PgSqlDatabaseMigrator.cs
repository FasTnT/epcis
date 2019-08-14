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

        public async Task Migrate(CancellationToken cancellationToken) => await _unitOfWork.Execute(await UnzipCommand(SqlRequests.CreateDatabaseZipped), cancellationToken);
        public async Task Rollback(CancellationToken cancellationToken) => await _unitOfWork.Execute(await UnzipCommand(SqlRequests.DropDatabaseZipped), cancellationToken);

        private async Task<string> UnzipCommand(string zippedCommand)
        {
            using (var reader = new StreamReader(new GZipStream(new MemoryStream(Convert.FromBase64String(zippedCommand)), CompressionMode.Decompress)))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
