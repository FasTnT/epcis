using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.DatabaseSetup
{
    public abstract class DatabaseRequestHandler<T> : IRequestHandler<T> where T : IRequest
    {
        private readonly IDbConnection _connection;
        private readonly string _request;

        public DatabaseRequestHandler(IDbConnection connection, string request)
        {
            _connection = connection;
            _request = request;
        }

        public async Task<Unit> Handle(T request, CancellationToken cancellationToken)
        {
            var sqlCommand = await DatabaseUtils.UnzipCommand(_request);
            var command = new CommandDefinition(sqlCommand, cancellationToken: cancellationToken);

            await _connection.ExecuteAsync(command);

            return Unit.Value;
        }
    }
}
