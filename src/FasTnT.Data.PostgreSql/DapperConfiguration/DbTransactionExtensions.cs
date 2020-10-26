using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Data.PostgreSql.DTOs.Subscriptions;
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
                { nameof(EpcDto), SqlQueries.Store_EpcDto },
                { nameof(EventDto), SqlQueries.Store_EventDto },
                { nameof(RequestDto), SqlQueries.Store_RequestDto },
                { nameof(SourceDestDto), SqlQueries.Store_SourceDestDto },
                { nameof(TransactionDto), SqlQueries.Store_TransactionDto },
                { nameof(CustomFieldDto), SqlQueries.Store_CustomFieldDto },
                { nameof(CorrectiveIdDto), SqlQueries.Store_CorrectiveIdDto},
                { nameof(StandardHeaderDto), SqlQueries.Store_StandardHeaderDto},
                { nameof(ContactInformationDto), SqlQueries.Store_ContactInformationDto},
                { nameof(SubscriptionCallbackDto), SqlQueries.Store_SubscriptionCallbackDto},
                { nameof(SubscriptionDto), SqlQueries.Store_SubscriptionDto },
                { nameof(SubscriptionInitialRequestDto), SqlQueries.Store_SubscriptionInitialRequestDto },
                { nameof(ParameterDto), SqlQueries.Store_ParameterDto },
                { nameof(ParameterValueDto), SqlQueries.Store_ParameterValueDto },
                // Masterdata DTOs
                { nameof(MasterDataDto), SqlQueries.Store_MasterDataDto },
                { nameof(MasterDataFieldDto), SqlQueries.Store_MasterDataFieldDto },
                { nameof(MasterDataAttributeDto), SqlQueries.Store_MasterDataAttributeDto },
                { nameof(MasterDataHierarchyDto), SqlQueries.Store_MasterDataHierarchyDto },
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
                batchSize: 5,
                cancellationToken: cancellationToken
            );
        }
    }
}
