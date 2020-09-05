using Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Domain.Data;
using FasTnT.Model.Users;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Data.PostgreSql.Query
{
    public class UserManager : IUserManager
    {
        private readonly IDbConnection _connection;

        public UserManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User> GetByUsername(string username, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlQueries.Read_UserByUsername, new { Username = username }, cancellationToken: cancellationToken);
            return await _connection.QuerySingleOrDefaultAsync<User>(command);
        }
    }
}
