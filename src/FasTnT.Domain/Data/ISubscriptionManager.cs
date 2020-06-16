using FasTnT.Domain.Model.Subscriptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface ISubscriptionManager
    {
        Task Store(Subscription subscription, CancellationToken cancellationToken);
        Task<IEnumerable<Subscription>> GetAll(CancellationToken cancellationToken);
        Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken);
        Task Delete(string subscriptionId, CancellationToken cancellationToken);
        Task<int[]> GetPendingRequestIds(string subscriptionId, CancellationToken cancellationToken);
        Task AcknowledgePendingRequests(string subscriptionId, int[] pendingRequests, CancellationToken cancellationToken);
        Task RegisterSubscriptionTrigger(string subscriptionId, SubscriptionResult success, string reason, CancellationToken cancellationToken);
    }
}
