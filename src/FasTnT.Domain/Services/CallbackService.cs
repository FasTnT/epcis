using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Users;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CallbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserContext _userContext;

        public CallbackService(IUnitOfWork unitOfWork, UserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task Process(EpcisQueryCallbackDocument result, CancellationToken cancellationToken)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var headerId = await tx.RequestStore.Store(result.Header, _userContext.Current, cancellationToken);

                await tx.CallbackStore.Store(headerId, result.SubscriptionName, QueryCallbackType.Success, cancellationToken);
                await tx.EventStore.Store(headerId, result.EventList, cancellationToken);
            });
        }

        public async Task ProcessException(EpcisQueryCallbackException result, CancellationToken cancellationToken)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var id = await tx.RequestStore.Store(result.Header, _userContext.Current, cancellationToken);

                await tx.CallbackStore.Store(id, result.SubscriptionName, result.CallbackType, cancellationToken);
            });
        }
    }
}
