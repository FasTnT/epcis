using FasTnT.Domain.Model.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface ISubscriptionManager
    {
        Task<string[]> GetSubscriptionIds();
        Task<Subscription[]> GetAll(CancellationToken cancellationToken);
        Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken);
        Task Delete(int subscriptionId, CancellationToken cancellationToken);
        Task<int[]> GetPendingRequestIds(int subscriptionId, CancellationToken cancellationToken);
        Task AcknowledgePendingRequests(int subscriptionId, int[] pendingRequests, CancellationToken cancellationToken);
        Task RegisterSubscriptionTrigger(int subscriptionId, SubscriptionResult success, string reason, CancellationToken cancellationToken);
    }
}
