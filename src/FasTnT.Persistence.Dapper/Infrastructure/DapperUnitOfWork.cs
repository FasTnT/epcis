using Dapper;
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

        public DapperUnitOfWork(IDbConnection connection) => _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        public void Begin()
        {
            if (_transaction != null || _hasCommitted) throw new Exception("This UnitOfWork instance has already been disposed.");

            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                if (!_hasCommitted) _transaction.Commit();
            }
            finally
            {
                _hasCommitted = true;
                _connection?.Close();
            }
        }

        public async Task Execute(string command, object parameters) => await _connection.ExecuteAsync(command, parameters, _transaction);
        public async Task<GridReader> FetchMany(string command, object parameters) =>  await _connection.QueryMultipleAsync(command, parameters, _transaction);
        public async Task<IEnumerable<T>> Query<T>(string command, object parameters) => await _connection.QueryAsync<T>(command, parameters, _transaction);
    }
}
