using FasTnT.Domain.Data;
using System.Data;

namespace FasTnT.PostgreSql
{
    public class TransactionProvider : ITransactionProvider
    {
        private readonly IDbConnection _connection;
        private bool _hasStarted;

        public TransactionProvider(IDbConnection connection)
        {
            _connection = connection;
        }

        public IDbTransaction BeginTransaction()
        {
            if (_hasStarted)
            {
                throw new System.Exception("A transaction can only be created once in a request");
            }

            var transaction = _connection.BeginTransaction();
            _hasStarted = true;

            return transaction;
        }
    }
}
