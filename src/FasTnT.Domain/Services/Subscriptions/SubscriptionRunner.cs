using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly IEpcisQuery[] _epcisQueries;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IEventRepository _eventRepository;
        private readonly ISubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEpcisQuery[] epcisQueries, ISubscriptionManager subscriptionManager, IEventRepository eventRepository, ISubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _subscriptionManager = subscriptionManager;
            _eventRepository = eventRepository;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription)
        {
            var query = GetQueryForSubscription(subscription);
            var response = new PollResponse { QueryName = query.Name, SubscriptionId = subscription.SubscriptionId };
            var pendingRequests = await _subscriptionManager.GetPendingRequestIds(subscription.Id);

            if (pendingRequests.Any())
            {
                _eventRepository.WhereSimpleFieldIn(EpcisField.RequestId, pendingRequests.ToArray());
                response.Entities = await query.Execute(subscription.Parameters, _eventRepository);
            }

            await SendSubscriptionResults(subscription, response);
            await _subscriptionManager.AcknowledgePendingRequests(subscription.Id, pendingRequests);
        }

        private async Task SendSubscriptionResults(Subscription subscription, PollResponse response)
        {
            if (response.Entities.Count() > 0 || subscription.ReportIfEmpty)
            {
                await _resultSender.Send(subscription.Destination, response);
            }
        }

        private IEpcisQuery GetQueryForSubscription(Subscription subscription) => _epcisQueries.Single(x => x.Name == subscription.QueryName);
    }
}
