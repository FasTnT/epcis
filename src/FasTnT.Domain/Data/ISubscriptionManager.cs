using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface ISubscriptionManager
    {
        Task<string[]> GetSubscriptionIds();
        Subscription GetSubscriptionById(string subscriptionId);
        Task DeleteSubscription(int subscriptionId);
    }
}
