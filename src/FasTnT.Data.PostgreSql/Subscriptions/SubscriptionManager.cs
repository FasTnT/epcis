using System.Threading.Tasks;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Data.PostgreSql.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        public Task DeleteSubscription(int subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Subscription GetSubscriptionById(string subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string[]> GetSubscriptionIds()
        {
            // TODO: implement
            return Task.FromResult(new string[0]);
        }
    }
}
