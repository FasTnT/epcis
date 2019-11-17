using System.Threading;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlCallbackManager : ICallbackManager
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlCallbackManager(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(int? requestId, string subscriptionId, QueryCallbackType callbackType, CancellationToken cancellationToken)
        {
            await _unitOfWork.Execute(PgSqlCallbackRequests.Store, new { RequestId = requestId, SubscriptionId = subscriptionId, CallbackType = callbackType }, cancellationToken);
        }
    }
}
