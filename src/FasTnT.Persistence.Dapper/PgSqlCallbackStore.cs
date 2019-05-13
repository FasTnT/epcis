using System;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlCallbackStore : ICallbackStore
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlCallbackStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(Guid? requestId, string subscriptionId, QueryCallbackType callbackType, CancellationToken cancellationToken)
        {
            await _unitOfWork.Execute(SqlRequests.StoreQueryCallback, new { Id = Guid.NewGuid(), RequestId = requestId, SubscriptionId = subscriptionId, CallbackType = callbackType }, cancellationToken);
        }
    }
}
