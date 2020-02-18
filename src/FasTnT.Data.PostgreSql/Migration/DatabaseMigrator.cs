using Dapper;
using FasTnT.Data.PostgreSql.Migration;
using FasTnT.Domain.Data;
using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Migration
{
    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly IDbConnection _connection;

        public DatabaseMigrator(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Migrate() => await Execute(DatabaseSqlRequests.CreateZipped);
        public async Task Rollback() => await Execute(DatabaseSqlRequests.DropZipped);

        private async Task Execute(string zippedCommand)
        {
            using (var reader = new StreamReader(new GZipStream(new MemoryStream(Convert.FromBase64String(zippedCommand)), CompressionMode.Decompress)))
            {
                var unzippedCommand = await reader.ReadToEndAsync();

                await _connection.ExecuteAsync(unzippedCommand);
            }
        }
    }
}
