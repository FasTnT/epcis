using System.Threading.Tasks;
using FasTnT.Domain.Data;

namespace FasTnT.Data.PostgreSql.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        public Task<string[]> GetSubscriptionIds()
        {
            // TODO: implement
            return Task.FromResult(new string[0]);
        }
    }
}
