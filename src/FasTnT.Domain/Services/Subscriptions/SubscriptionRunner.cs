using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly IEpcisQuery[] _epcisQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEpcisQuery[] epcisQueries, IUnitOfWork unitOfWork, ISubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _unitOfWork = unitOfWork;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription, CancellationToken cancellationToken)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var query = _epcisQueries.Single(x => x.Name == subscription.QueryName);
                var response = new PollResponse { QueryName = query.Name, SubscriptionId = subscription.SubscriptionId };
                try
                {

                    var pendingRequests = await tx.SubscriptionManager.GetPendingRequestIds(subscription.SubscriptionId, cancellationToken);

                    if (pendingRequests.Any())
                    {
                        tx.EventManager.WhereSimpleFieldIn(EpcisField.RequestId, pendingRequests.ToArray());
                        response.Entities = await query.Execute(subscription.Parameters, tx, cancellationToken);
                    }

                    await SendSubscriptionResults(subscription, response);
                    await tx.SubscriptionManager.AcknowledgePendingRequests(subscription.SubscriptionId, pendingRequests, cancellationToken);
                    await tx.SubscriptionManager.RegisterSubscriptionTrigger(subscription.SubscriptionId, SubscriptionResult.Success, default, cancellationToken);
                }
                catch (Exception ex)
                {
                    await tx.SubscriptionManager.RegisterSubscriptionTrigger(subscription.SubscriptionId, SubscriptionResult.Failed, ex.Message, cancellationToken);
                }
            });
        }

        private async Task SendSubscriptionResults(Subscription subscription, PollResponse response)
        {
            if (response.Entities.Count() > 0 || subscription.ReportIfEmpty)
            {
                await _resultSender.Send(subscription.Destination, response);
            }
        }
    }
}
