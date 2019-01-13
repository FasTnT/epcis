using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlSubscriptionManager : ISubscriptionManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PgSqlSubscriptionManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Subscription>> GetAll(bool withDetails = false)
        {
            var subscriptions = await _unitOfWork.Query<Subscription>(SqlRequests.ListSubscriptionIds, new { QueryName = "SimpleEventQuery" });

            if (withDetails)
            {
                var controls = default(IEnumerable<SubscriptionControls>);
                var queryParameters = default(IEnumerable<QueryParameter>);

                foreach (var subscription in subscriptions)
                {
                    subscription.Controls = controls.SingleOrDefault(x => x.SubscriptionId == subscription.Id);
                    subscription.Params = queryParameters;
                }
            }

            return subscriptions;
        }

        public async Task<IEnumerable<Subscription>> ListForQuery(string queryName) => await _unitOfWork.Query<Subscription>(SqlRequests.ListSubscriptionIds, new { QueryName = queryName });
        public Task Store(Subscription subscription) => throw new NotImplementedException();
        public async Task Delete(Guid id) => await _unitOfWork.Execute(SqlRequests.DeleteSubscription, new { Id = id });
    }
}
