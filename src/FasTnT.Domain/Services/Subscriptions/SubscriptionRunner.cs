using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEpcisQuery[] epcisQueries, IUnitOfWork unitOfWork, ISubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _unitOfWork = unitOfWork;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription)
        {
            await _unitOfWork.Execute(async tx =>
            {
                var query = GetQueryForSubscription(subscription);
                var response = new PollResponse { QueryName = query.Name, SubscriptionId = subscription.SubscriptionId };
                var pendingRequests = await tx.SubscriptionManager.GetPendingRequestIds(subscription.Id);

                if (pendingRequests.Any())
                {
                    tx.EventManager.WhereSimpleFieldIn(EpcisField.RequestId, pendingRequests.ToArray());
                    response.Entities = await query.Execute(subscription.Parameters, tx);
                }

                await SendSubscriptionResults(subscription, response);
                await tx.SubscriptionManager.AcknowledgePendingRequests(subscription.Id, pendingRequests);
            });
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
