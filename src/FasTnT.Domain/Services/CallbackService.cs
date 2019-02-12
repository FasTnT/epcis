using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CallbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CallbackService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Process(EpcisQueryCallbackDocument result)
        {
            await _unitOfWork.Execute(async tx =>
            {
                await tx.RequestStore.Store(result.Header);
                await tx.CallbackStore.Store(result.Header.Id, result.SubscriptionName, QueryCallbackType.Success);
                await tx.EventStore.Store(result.Header.Id, result.EventList);
            });
        }

        public async Task ProcessException(QueryCallbackType callbackType)
        {
            await _unitOfWork.Execute(async tx =>
            {
                await tx.CallbackStore.Store(null, "unknown", callbackType);
            });
        }
    }
}
