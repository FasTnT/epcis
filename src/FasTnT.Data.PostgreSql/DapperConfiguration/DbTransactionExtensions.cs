using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.DapperConfiguration
{
    internal static class DbTransactionExtensions
    {
        private readonly static IDictionary<string, string> _insertCommands =
            new Dictionary<string, string>
            {
                { nameof(EpcDto), CaptureSqlQueries.Store_EpcDto },
                { nameof(EventDto), CaptureSqlQueries.Store_EventDto },
                { nameof(RequestDto), CaptureSqlQueries.Store_RequestDto },
                { nameof(SourceDestDto), CaptureSqlQueries.Store_SourceDestDto },
                { nameof(TransactionDto), CaptureSqlQueries.Store_TransactionDto },
                { nameof(CustomFieldDto), CaptureSqlQueries.Store_CustomFieldDto },
                { nameof(CorrectiveIdDto), CaptureSqlQueries.Store_CorrectiveIdDto},
                { nameof(StandardHeaderDto), CaptureSqlQueries.Store_StandardHeaderDto},
                { nameof(ContactInformationDto), CaptureSqlQueries.Store_ContactInformationDto},
                { nameof(SubscriptionCallbackDto), CaptureSqlQueries.Store_SubscriptionCallbackDto}
            };

        public static async Task<int> InsertAsync<T>(this IDbTransaction transaction, T entity, CancellationToken cancellationToken = default)
        {
            var connection = transaction.Connection;
            var command = new CommandDefinition(
                commandText: _insertCommands[typeof(T).Name],
                parameters: entity,
                transaction: transaction,
                commandTimeout: 10,
                cancellationToken: cancellationToken
            );

            return await connection.ExecuteScalarAsync<int>(command);
        }

        public static async Task BulkInsertAsync<T>(this IDbTransaction transaction, IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var connection = transaction.Connection;

            await connection.BulkInsertAsync(
                sql: _insertCommands[typeof(T).Name],
                insertParams: entities,
                transaction: transaction,
                batchSize: 1000,
                cancellationToken: cancellationToken
            );
        }
    }
}
