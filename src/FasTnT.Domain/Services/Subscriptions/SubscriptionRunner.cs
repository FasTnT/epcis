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
            using (new CommitOnDispose(_unitOfWork))
            {
                var query = GetQueryForSubscription(subscription);
                var response = new PollResponse { QueryName = query.Name, SubscriptionId = subscription.SubscriptionId };
                var pendingRequests = await _unitOfWork.SubscriptionManager.GetPendingRequestIds(subscription.Id);

                if (pendingRequests.Any())
                {
                    _unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.RequestId, pendingRequests.ToArray());
                    response.Entities = await query.Execute(subscription.Parameters, _unitOfWork);
                }

                await SendSubscriptionResults(subscription, response);
                await _unitOfWork.SubscriptionManager.AcknowledgePendingRequests(subscription.Id, pendingRequests);
            }
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
