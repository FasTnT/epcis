using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly IEpcisQuery[] _epcisQueries;
        private readonly IEventRepository _eventRepository;
        private readonly ISubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEpcisQuery[] epcisQueries, IEventRepository eventRepository, ISubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _eventRepository = eventRepository;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription)
        {
            var query = GetQueryForSubscription(subscription);
            var pendingRequests = new object[] { Guid.NewGuid() };
            _eventRepository.WhereSimpleFieldIn(EpcisField.RequestId, pendingRequests);

            var response = new PollResponse {
                QueryName = query.Name,
                SubscriptionId = subscription.SubscriptionId,
                Entities = await query.Execute(subscription.Parameters, _eventRepository)
            };

            await SendSubscriptionResults(subscription, response);
        }

        private async Task SendSubscriptionResults(Subscription subscription, PollResponse response)
        {
            if (response.Entities.Count() > 0 || subscription.ReportIfEmpty)
            {
                await _resultSender.Send(subscription.Destination, default(IEpcisResponse));
            }
        }

        private IEpcisQuery GetQueryForSubscription(Subscription subscription) => _epcisQueries.Single(x => x.Name == subscription.QueryName);
    }
}
