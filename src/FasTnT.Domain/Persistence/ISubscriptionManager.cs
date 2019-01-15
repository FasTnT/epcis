using FasTnT.Model.Subscriptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ISubscriptionManager
    {
        Task<Subscription> GetById(string subscriptionId);
        Task<IEnumerable<Subscription>> GetAll(bool withDetails = false);
        Task Store(Subscription subscription);
        Task Delete(Guid id);
        Task<IEnumerable<Guid>> GetPendingRequestIds(Guid subscriptionId);
        Task AcknowledgePendingRequests(Guid subscriptionId, IEnumerable<Guid> requestIds);
    }
}
