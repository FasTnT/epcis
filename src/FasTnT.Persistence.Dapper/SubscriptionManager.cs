using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using System;
using System.Collections.Generic;
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
            var subscriptions = await _unitOfWork.Query<Subscription>(SqlRequests.ListSubscriptions);

            if (withDetails)
            {
                var queryParameters = default(IEnumerable<QueryParameter>);

                foreach (var subscription in subscriptions)
                {
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
