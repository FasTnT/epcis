using FasTnT.Model.Subscriptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ISubscriptionManager
    {
        Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken);
        Task<IEnumerable<Subscription>> GetAll(bool withDetails, CancellationToken cancellationToken);
        Task Store(Subscription subscription, CancellationToken cancellationToken);
        Task Delete(string subscriptionId, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetPendingRequestIds(string subscriptionId, CancellationToken cancellationToken);
        Task AcknowledgePendingRequests(string subscriptionId, IEnumerable<Guid> requestIds, CancellationToken cancellationToken);
    }
}
