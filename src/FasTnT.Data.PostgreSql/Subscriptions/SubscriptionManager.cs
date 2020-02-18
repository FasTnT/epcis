using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Model.Queries;
using MoreLinq;

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
            var entity = new
            {
                Active = true,
                subscription.SubscriptionId,
                subscription.Trigger,
                subscription.InitialRecordTime,
                subscription.ReportIfEmpty,
                subscription.Schedule?.Second,
                subscription.Schedule?.Hour,
                subscription.Schedule?.Minute,
                subscription.Schedule?.Month,
                subscription.Schedule?.DayOfMonth,
                subscription.Schedule?.DayOfWeek,
                subscription.Destination,
                subscription.QueryName
            };

            var entityId = await _connection.ExecuteScalarAsync<int>(new CommandDefinition(SubscriptionRequests.Store, entity, cancellationToken: cancellationToken));
            var parameters = subscription.Parameters.Select(parameter => new { SubscriptionId = entityId, parameter.Name }).ToArray();
            var parameterIds = (await _connection.BulkInsertReturningAsync(SubscriptionRequests.StoreParameter, parameters, cancellationToken: cancellationToken)).ToArray();
            var values = subscription.Parameters.SelectMany((p, i) => p.Values.Select(value => new { ParameterId = parameterIds[i], Value = value })).ToArray();

            await _connection.BulkInsertAsync(SubscriptionRequests.StoreParameterValue, values, cancellationToken: cancellationToken);
        }

        public async Task AcknowledgePendingRequests(int subscriptionId, int[] requestIds, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.AcknowledgePendingRequests, new { subscriptionId, requestIds }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public async Task Delete(int subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.DeleteSubscription, new { subscriptionId }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public async Task<int[]> GetPendingRequestIds(int subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.ListPendingRequests, new { subscriptionId }, cancellationToken: cancellationToken);
            var pendingRequests = await _connection.QueryAsync<int>(command);

            return pendingRequests.ToArray();
        }

        public async Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.GetSubscriptionById, new { subscriptionId }, cancellationToken: cancellationToken);
            var subscription = await _connection.QueryAsync<Subscription>(command);

            await LoadParameters(subscription, cancellationToken);

            return subscription.SingleOrDefault();
        }

        public async Task RegisterSubscriptionTrigger(int subscriptionId, SubscriptionResult result, string reason, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.RegisterTrigger, new { subscriptionId, result, reason }, cancellationToken: cancellationToken);

            await _connection.ExecuteAsync(command);
        }

        public async Task<string[]> GetSubscriptionIds(CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.ListSubscriptionIds, cancellationToken: cancellationToken);
            var subscriptionIds = await _connection.QueryAsync<string>(command);

            return subscriptionIds.ToArray();
        }

        public async Task<Subscription[]> GetAll(CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.GetAllSubscriptions, cancellationToken: cancellationToken);
            var subscriptions = (await _connection.QueryAsync<dynamic>(command)).Select(x => new Subscription
            {
                Id = x.id,
                Active = x.active,
                Destination = x.destination,
                QueryName = x.query_name,
                Trigger = x.trigger,
                SubscriptionId = x.subscription_id,
                ReportIfEmpty = x.report_if_empty,
                Schedule = new QuerySchedule
                {
                    DayOfMonth = x.schedule_day_of_month,
                    DayOfWeek = x.schedule_day_of_week,
                    Hour = x.schedule_hours,
                    Minute = x.schedule_minutes,
                    Month = x.schedule_month,
                    Second = x.schedule_seconds
                }
            });

            await LoadParameters(subscriptions, cancellationToken);

            return subscriptions.ToArray();
        }

        private async Task LoadParameters(IEnumerable<Subscription> subscriptions, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.ListParameters, new { SubscriptionIds = subscriptions.Select(x => x.Id.Value).ToArray() }, cancellationToken: cancellationToken);
            var @params = (await _connection.QueryAsync<dynamic>(command))
                .GroupBy(x => (int) x.subscription_id)
                .Select(x => new
                {
                    SubscriptionId = x.Key,
                    Parameters = x.GroupBy(g => (string)g.name).Select(p => new QueryParameter
                    {
                        Name = p.Key,
                        Values = p.Select(v => (string)v.value).ToArray()
                    }).ToArray()
                }).ToArray();

            subscriptions.ForEach(s => s.Parameters = @params.SingleOrDefault(p => p.SubscriptionId == s.Id)?.Parameters);
        }
    }
}
