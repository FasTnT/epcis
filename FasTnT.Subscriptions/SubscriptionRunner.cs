using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public class SubscriptionRunner
    {
        public static Assembly Assembly = typeof(SubscriptionRunner).Assembly;

        private readonly IEnumerable<IEpcisQuery> _epcisQueries;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly SubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEnumerable<IEpcisQuery> epcisQueries, ISubscriptionManager subscriptionManager, SubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _subscriptionManager = subscriptionManager;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription, CancellationToken cancellationToken)
        {
            var query = _epcisQueries.Single(x => x.Name == subscription.QueryName);
            var response = new PollResponse();

            try
            {
                var pendingRequests = await _subscriptionManager.GetPendingRequestIds(subscription.Id.Value, cancellationToken);

                if (pendingRequests.Any())
                {
                    var parameters = subscription.Parameters.Append(new Model.Queries.QueryParameter { Name = "EQ_requestId", Values = pendingRequests.Select(x => x.ToString()).ToArray() });
                    response = await query.Handle(parameters.ToArray(), cancellationToken);
                    response.QueryName = query.Name;
                    response.SubscriptionId = subscription.SubscriptionId;
                }

                await SendSubscriptionResults(subscription, response, cancellationToken);
                await _subscriptionManager.AcknowledgePendingRequests(subscription.Id.Value, pendingRequests, cancellationToken);
                await _subscriptionManager.RegisterSubscriptionTrigger(subscription.Id.Value, SubscriptionResult.Success, default, cancellationToken);
            }
            catch (Exception ex)
            {
                await _subscriptionManager.RegisterSubscriptionTrigger(subscription.Id.Value, SubscriptionResult.Failed, ex.Message, cancellationToken);
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
