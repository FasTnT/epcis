using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class QueryService
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionBackgroundService _backgroundService;

        public QueryService(IEpcisQuery[] queries, IUnitOfWork unitOfWork, ISubscriptionBackgroundService backgroundService)
        {
            _queries = queries;
            _unitOfWork = unitOfWork;
            _backgroundService = backgroundService;
        }

        public Task<GetQueryNamesResponse> GetQueryNames() => Task.Run(() => new GetQueryNamesResponse { QueryNames = _queries.Select(x => x.Name) });
        public Task<GetStandardVersionResponse> GetStandardVersion() => Task.Run(() => new GetStandardVersionResponse { Version = Constants.StandardVersion });
        public Task<GetVendorVersionResponse> GetVendorVersion() => Task.Run(() => new GetVendorVersionResponse { Version = Constants.ProductVersion });
        public async Task<GetSubscriptionIdsResult> GetSubscriptionId(GetSubscriptionIds query) => new GetSubscriptionIdsResult { SubscriptionIds = (await _unitOfWork.SubscriptionManager.GetAll()).Where(s => s.QueryName == query.QueryName).Select(x => x.SubscriptionId) };

        public async Task<PollResponse> Poll(Poll query)
        {
            var epcisQuery = _queries.SingleOrDefault(x => x.Name == query.QueryName);

            if (epcisQuery == null)
            {
                throw new Exception($"Unknown query: '{query.QueryName}'");
            }
            else
            {
                query.Parameters = query.Parameters.Where(x => !x.Values.All(string.IsNullOrEmpty));
                try
                {
                    epcisQuery.ValidateParameters(query.Parameters);

                    var results = await epcisQuery.Execute(query.Parameters, _unitOfWork);
                    return new PollResponse { QueryName = query.QueryName, Entities = results };
                }
                catch (Exception ex)
                {
                    throw new EpcisException(ExceptionType.QueryParameterException, ex.Message);
                }
            }
        }

        public async Task Subscribe(Subscription request)
        {
            EnsureDestinationIsValidURI(request);
            EnsureQueryAllowsSubscription(request);
            EnsureDestinationHasEndSlash(request);
            request.Parameters = request.Parameters.Where(x => !x.Values.All(string.IsNullOrEmpty));

            await _unitOfWork.Execute(async tx =>
            {
                await EnsureSubscriptionDoesNotExist(tx, request);
                await tx.SubscriptionManager.Store(request);
                _backgroundService.Register(request);
            });
        }

        public async Task Unsubscribe(UnsubscribeRequest query)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var subscription = await tx.SubscriptionManager.GetById(query.SubscriptionId);
                if (subscription == null) throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription with ID '{query.SubscriptionId}' does not exist.");

                await tx.SubscriptionManager.Delete(subscription.SubscriptionId);
                _backgroundService.Remove(subscription);
            });
        }

        private void EnsureDestinationHasEndSlash(Subscription request) => request.Destination = $"{request.Destination.TrimEnd('/')}/";
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
