using FasTnT.Domain.Commands.Setup;
using System.Data;

namespace FasTnT.Handlers.DatabaseSetup
{
    public class RollbackDatabaseHandler : DatabaseRequestHandler<RollbackDatabaseRequest>
    {
        public RollbackDatabaseHandler(IDbConnection connection) : base(connection, DatabaseSqlRequests.DropZipped) { }
    }
}
