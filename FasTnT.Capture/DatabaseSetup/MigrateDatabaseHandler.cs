using FasTnT.Domain.Commands.Setup;
using System.Data;

namespace FasTnT.Handlers.DatabaseSetup
{
    public class MigrateDatabaseHandler : DatabaseRequestHandler<MigrateDatabaseRequest>
    {
        public MigrateDatabaseHandler(IDbConnection connection) : base(connection, DatabaseSqlRequests.CreateZipped) { }
    }
}
