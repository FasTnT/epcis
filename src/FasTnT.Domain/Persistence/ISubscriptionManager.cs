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
        Task Delete(string subscriptionId);
        Task<IEnumerable<Guid>> GetPendingRequestIds(string subscriptionId);
        Task AcknowledgePendingRequests(string subscriptionId, IEnumerable<Guid> requestIds);
    }
}
