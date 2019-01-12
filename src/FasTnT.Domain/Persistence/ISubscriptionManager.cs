using FasTnT.Model.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ISubscriptionManager
    {
        Task<IEnumerable<Subscription>> GetAll(bool withDetails = false);
        Task<IEnumerable<Subscription>> ListForQuery(string queryName);
        Task Store(Subscription subscription);
    }
}
