using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Data.PostgreSql.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IDbConnection _connection;

        public SubscriptionManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AcknowledgePendingRequests(int subscriptionId, int[] pendingRequests, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.AcknowledgePendingRequests, new { subscriptionId, pendingRequests }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public async Task Delete(int subscriptionId, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.DeleteSubscription, new { subscriptionId }, cancellationToken: cancellationToken);
            await _connection.ExecuteAsync(command);
        }

        public Task<int[]> GetPendingRequestIds(int subscriptionId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken)
        {
            // TODO
            return Task.FromResult(new Subscription());
        }

        public async Task RegisterSubscriptionTrigger(int subscriptionId, SubscriptionResult result, string reason, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(SubscriptionRequests.RegisterTrigger, new { subscriptionId, result, reason }, cancellationToken: cancellationToken);

            await _connection.ExecuteAsync(command);
        }

        public Task<string[]> GetSubscriptionIds()
        {
            // TODO: implement
            return Task.FromResult(new string[0]);
        }

        public Task<Subscription[]> GetAll(CancellationToken cancellationToken)
        {
            // TODO
            return Task.FromResult(new Subscription[0]);
        }
    }
}
