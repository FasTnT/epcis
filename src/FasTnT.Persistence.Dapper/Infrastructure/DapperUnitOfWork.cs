using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Setup;
using FasTnT.Persistence.Dapper.Setup;
using System;
using System.Collections.Generic;
using System.Data;
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
            EventStore = new EventStore(this);
            EventManager = new PgSqlEventRepository(this);
            SubscriptionManager = new PgSqlSubscriptionManager(this);
            DatabaseManager = new PgSqlDatabaseMigrator(this);
        }

        public IEventStore EventStore { get; }
        public IEventRepository EventManager { get; }
        public ISubscriptionManager SubscriptionManager { get; }
        public IMasterDataManager MasterDataManager { get; }
        public IDatabaseMigrator DatabaseManager { get; }

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

        public async Task Execute(string command, object parameters = null) => await _connection.ExecuteAsync(command, parameters, _transaction);
        public async Task<GridReader> FetchMany(string command, object parameters = null) =>  await _connection.QueryMultipleAsync(command, parameters, _transaction);
        public async Task<IEnumerable<T>> Query<T>(string command, object parameters = null) => await _connection.QueryAsync<T>(command, parameters, _transaction);
    }
}
