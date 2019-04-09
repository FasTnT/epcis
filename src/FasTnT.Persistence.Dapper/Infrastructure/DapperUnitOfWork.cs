using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Setup;
using FasTnT.Persistence.Dapper.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace FasTnT.Persistence.Dapper
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _hasCommitted;

        public DapperUnitOfWork(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            EventStore = new PgSqlEventStore(this);
            CallbackStore = new PgSqlCallbackStore(this);
            RequestStore = new PgSqlRequestStore(this);
            EventManager = new PgSqlEventRepository(this);
            SubscriptionManager = new PgSqlSubscriptionManager(this);
            DatabaseManager = new PgSqlDatabaseMigrator(this);
            MasterDataManager = new PgSqlMasterDataManager(this);
            UserManager = new PgSqlUserManager(this);
        }

        public ICallbackStore CallbackStore { get; }
        public IRequestStore RequestStore { get; }
        public IEventStore EventStore { get; }
        public IEventRepository EventManager { get; }
        public ISubscriptionManager SubscriptionManager { get; }
        public IMasterDataManager MasterDataManager { get; }
        public IDatabaseMigrator DatabaseManager { get; }
        public IUserManager UserManager { get; }


        public void BeginTransaction()
        {
            if (_transaction != null || _hasCommitted) throw new Exception("This UnitOfWork instance has already been disposed.");

            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit() => End(tx => tx.Commit());
        public void Rollback() => End(tx => tx.Rollback());

        private void End(Action<IDbTransaction> action)
        {
            try
            {
                if (!_hasCommitted) action(_transaction);
            }
            finally
            {
                _hasCommitted = true;
                _connection?.Close();
            }
        }

        public async Task Execute(string command, object parameters = null, CancellationToken cancellationToken = default) => await _connection.ExecuteAsync(new CommandDefinition(command, parameters, _transaction, cancellationToken: cancellationToken));
        public async Task<GridReader> FetchMany(string command, object parameters = null, CancellationToken cancellationToken = default) =>  await _connection.QueryMultipleAsync(new CommandDefinition(command, parameters, _transaction, cancellationToken: cancellationToken));
        public async Task<IEnumerable<T>> Query<T>(string command, object parameters = null, CancellationToken cancellationToken = default) => await _connection.QueryAsync<T>(new CommandDefinition(command, parameters, _transaction, cancellationToken: cancellationToken));
    }
}
