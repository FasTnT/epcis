using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Subscriptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class SubscriptionService
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public SubscriptionService(IEpcisQuery[] queries, IUnitOfWork unitOfWork, ISubscriptionBackgroundService backgroundService)
        {
            _queries = queries;
            _unitOfWork = unitOfWork;
            _backgroundService = backgroundService;
        }

        public async Task Process(Subscription request)
        {
            EnsureDestinationIsValidURI(request);
            EnsureQueryAllowsSubscription(request);
            EnsureDestinationHasEndSlash(request);

            await _unitOfWork.Execute(async tx =>
            {
                await EnsureSubscriptionDoesNotExist(tx, request);
                await tx.SubscriptionManager.Store(request);
                _backgroundService.Register(request);
            });
        }

        private void EnsureDestinationHasEndSlash(Subscription request) => request.Destination = $"{request.Destination.TrimEnd('/')}/";

        public async Task Process(UnsubscribeRequest query)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var subscription = await tx.SubscriptionManager.GetById(query.SubscriptionId);
                if (subscription == null) throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription with ID '{query.SubscriptionId}' does not exist.");

                await tx.SubscriptionManager.Delete(subscription.Id);
                _backgroundService.Remove(subscription);
            });
        }

        public Task Process(TriggerSubscriptionRequest query) => Task.Run(() => _backgroundService.Trigger(query.Trigger));

        private void EnsureDestinationIsValidURI(Subscription request) => UriValidator.Validate(request.Destination, true);

        private async static Task EnsureSubscriptionDoesNotExist(IUnitOfWork transaction, Subscription request)
        {
            var subscription = await transaction.SubscriptionManager.GetById(request.SubscriptionId);
            if (subscription != null) throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Subscription '{request.QueryName}' already exist.");
        }

        private void EnsureQueryAllowsSubscription(Subscription subscribe)
        {
            var query = _queries.SingleOrDefault(x => x.Name == subscribe.QueryName);
            if (query == null || !query.AllowSubscription) throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscribe.QueryName}' does not exist or doesn't allow subscription");
        }
    }
}
