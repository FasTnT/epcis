using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Subscriptions
{
    public class UnsubscribeHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public UnsubscribeHandler(IUnitOfWork unitOfWork, ISubscriptionBackgroundService backgroundService)
        {
            _unitOfWork = unitOfWork;
            _backgroundService = backgroundService;
        }

        public async Task<IEpcisResponse> Handle(UnsubscribeRequest query)
        {
            using (new CommitOnDispose(_unitOfWork))
            {
                var subscription = await _unitOfWork.SubscriptionManager.GetById(query.SubscriptionId);

                if (subscription == null)
                {
                    throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription with ID '{query.SubscriptionId}' does not exist.");
                }

                await _unitOfWork.SubscriptionManager.Delete(subscription.Id);
                _backgroundService.Remove(subscription);

                return new UnsubscribeResponse();
            }
        }
    }
}
