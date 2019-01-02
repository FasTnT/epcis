using FasTnT.Domain.Persistence;
using FasTnT.Model.Subscriptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper
{
    public class SubscriptionManager : ISubscriptionManager
    {
        public async Task<IEnumerable<Subscription>> ListAll()
        {
            throw new NotImplementedException();
        }
    }
}
