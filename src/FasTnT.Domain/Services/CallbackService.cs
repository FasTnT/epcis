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
                var headerId = await tx.RequestStore.Store(result.Header);
                await tx.CallbackStore.Store(headerId, result.SubscriptionName, QueryCallbackType.Success);
                await tx.EventStore.Store(headerId, result.EventList);
            });
        }

        public async Task ProcessException(EpcisQueryCallbackException result)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var id = await tx.RequestStore.Store(result.Header);
                await tx.CallbackStore.Store(id, result.SubscriptionName, result.CallbackType);
            });
        }
    }
}
