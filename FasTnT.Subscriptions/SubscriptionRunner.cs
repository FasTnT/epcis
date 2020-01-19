using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly IEpcisQuery[] _epcisQueries;
        private readonly IEventFetcher _eventFetcher;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly SubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEpcisQuery[] epcisQueries, IEventFetcher eventFetcher, ISubscriptionManager subscriptionManager, SubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _eventFetcher = eventFetcher;
            _subscriptionManager = subscriptionManager;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription, CancellationToken cancellationToken)
        {
            var query = _epcisQueries.Single(x => x.Name == subscription.QueryName);
            var response = default(PollResponse) ;
            try
            {
                //var pendingRequests = await _subscriptionManager.GetPendingRequestIds(subscription.SubscriptionId, cancellationToken);

                //if (pendingRequests.Any())
                //{
                //    _eventFetcher.Apply(new SimpleParameterFilter { Field = EpcisField.RequestId, Values = pendingRequests.ToArray() });
                //    response = await query.Handle(subscription.Parameters, cancellationToken);
                //    response.QueryName = query.Name;
                //    response.SubscriptionId = subscription.SubscriptionId;
                //}

                await SendSubscriptionResults(subscription, response, cancellationToken);
                //await _subscriptionManager.AcknowledgePendingRequests(subscription.SubscriptionId, pendingRequests, cancellationToken);
                //await _subscriptionManager.RegisterSubscriptionTrigger(subscription.SubscriptionId, SubscriptionResult.Success, default, cancellationToken);
            }
            catch (Exception ex)
            {
                //await tx.SubscriptionManager.RegisterSubscriptionTrigger(subscription.SubscriptionId, SubscriptionResult.Failed, ex.Message, cancellationToken);
            }
        }

        private async Task SendSubscriptionResults(Subscription subscription, PollResponse response, CancellationToken cancellationToken)
        {
            if (response.EventList.Count() > 0 || subscription.ReportIfEmpty)
            {
                await _resultSender.Send(subscription.Destination, response, cancellationToken);
            }
        }
    }
}
