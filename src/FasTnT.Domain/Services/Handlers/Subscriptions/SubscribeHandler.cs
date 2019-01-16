using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Subscriptions
{
    public class SubscribeHandler : ISubscriptionHandler<Subscription>
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public SubscribeHandler(IEpcisQuery[] queries, IUnitOfWork unitOfWork, ISubscriptionBackgroundService backgroundService)
        {
            _queries = queries;
            _unitOfWork = unitOfWork;
            _backgroundService = backgroundService;
        }

        public async Task<IEpcisResponse> Handle(Subscription request)
        {
            EnsureQueryAllowsSubscription(request);

            using (new CommitOnDispose(_unitOfWork))
            {
                await EnsureSubscriptionDoesNotExist(request);
                await _unitOfWork.SubscriptionManager.Store(request);
                _backgroundService.Register(request);

                return new SubscribeResponse();
            }
        }

        private async Task EnsureSubscriptionDoesNotExist(Subscription request)
        {
            var subscription = await _unitOfWork.SubscriptionManager.GetById(request.SubscriptionId);
            if (subscription != null)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Subscription '{request.QueryName}' already exist.");
            }
        }

        private void EnsureQueryAllowsSubscription(Subscription subscribe)
        {
            var query = _queries.SingleOrDefault(x => x.Name == subscribe.QueryName);

            if(query == null || !query.AllowSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscribe.QueryName}' does not exist or doesn't allow subscription");
            }
        }
    }
}
