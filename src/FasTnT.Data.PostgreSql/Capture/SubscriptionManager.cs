using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs.Subscriptions;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.PostgreSql.DapperConfiguration;

namespace FasTnT.Data.PostgreSql.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IDbConnection _connection;

        public SubscriptionManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Store(Subscription subscription, CancellationToken cancellationToken)
        {
            var parameters = new List<ParameterDto>();
            var parameterValues = new List<ParameterValueDto>();

            using var transaction = _connection.BeginTransaction();
            var subscriptionId = await transaction.InsertAsync(SubscriptionDto.Create(subscription));
            await transaction.InsertAsync(SubscriptionInitialRequestDto.Create(subscription, subscriptionId));

            for(short id=0; id<subscription.Parameters.Count; id++)
            {
                parameters.Add(ParameterDto.Create(subscription.Parameters[id], id, subscriptionId));
                parameterValues.AddRange(subscription.Parameters[id].Values.Select(x => ParameterValueDto.Create(x, id, subscriptionId)));
            }

            await transaction.BulkInsertAsync(parameters, cancellationToken);
            await transaction.BulkInsertAsync(parameterValues, cancellationToken);

            transaction.Commit();
        }

        public async Task AcknowledgePendingRequests(string subscriptionId, int[] requestIds, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.AcknowledgePendingRequests, new { subscriptionId, requestIds }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public async Task Delete(string subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.DeleteSubscription, new { subscriptionId }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public async Task<int[]> GetPendingRequestIds(string subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.ListPendingRequests, new { subscriptionId }, cancellationToken: cancellationToken);
            var pendingRequests = await _connection.QueryAsync<int>(command);

            return pendingRequests.ToArray();
        }

        public async Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.GetSubscriptionById, new { subscriptionId }, cancellationToken: cancellationToken);
            var reader = await _connection.QueryMultipleAsync(command);
            var manager = await SubscriptionDtoManager.ReadAsync(reader);

            return manager.FormatSubscriptions().FirstOrDefault();
        }

        public async Task RegisterSubscriptionTrigger(string subscriptionId, SubscriptionResult result, string reason, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.RegisterTrigger, new { subscriptionId, Result = result.Id, reason }, cancellationToken: cancellationToken);

            await _connection.ExecuteAsync(command);
        }

        public async Task<IEnumerable<Subscription>> GetAll(CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SqlSubscriptionQueries.GetAllSubscriptions, cancellationToken: cancellationToken);
            var reader = await _connection.QueryMultipleAsync(command);
            var manager = await SubscriptionDtoManager.ReadAsync(reader);

            return manager.FormatSubscriptions();
        }
    }
}
