using FasTnT.Model.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ISubscriptionManager
    {
        Task<IEnumerable<Subscription>> ListAll();
    }
}
