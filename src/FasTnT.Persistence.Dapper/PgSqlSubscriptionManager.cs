using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlSubscriptionManager : ISubscriptionManager
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlSubscriptionManager(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Subscription> GetById(string subscriptionId, CancellationToken cancellationToken)
        {
            return (await _unitOfWork.Query<Subscription>($"{PgSqlSubscriptionRequests.List} WHERE s.subscription_id = @Id", new { Id = subscriptionId }, cancellationToken)).SingleOrDefault();
        }

        public async Task<IEnumerable<Subscription>> GetAll(bool includeDetails, CancellationToken cancellationToken)
        {
            var subscriptions = (await _unitOfWork.Query<dynamic>(PgSqlSubscriptionRequests.List, null, cancellationToken)).Select(x => new SubscriptionEntity
            {
                Id = x.id,
                Active = x.active,
                Destination = x.destination,
                QueryName = x.query_name,
                Trigger = x.trigger,
                SubscriptionId = x.subscription_id,
                ReportIfEmpty = x.report_if_empty,
                Schedule = new QuerySchedule
                {
                    DayOfMonth = x.schedule_day_of_month,
                    DayOfWeek = x.schedule_day_of_week,
                    Hour = x.schedule_hours,
                    Minute = x.schedule_minutes,
                    Month = x.schedule_month,
                    Second = x.schedule_seconds
                }
            }).ToArray();

            if (includeDetails) await LoadParameters(subscriptions, cancellationToken);

            return subscriptions;
        }

        private async Task LoadParameters(SubscriptionEntity[] subscriptions, CancellationToken cancellationToken)
        {
            var @params = (await _unitOfWork.Query<dynamic>(PgSqlSubscriptionRequests.ListParameters, null, cancellationToken))
                .GroupBy(x => (int)x.subscription_id)
                .Select(x => new
                {
                    SubscriptionId = x.Key,
                    Parameters = x.GroupBy(g => (string)g.name).Select(p => new QueryParameter
                    {
                        Name = p.Key,
                        Values = p.Select(v => (string)v.value).ToArray()
                    }).ToArray()
                }).ToArray();

            subscriptions.ForEach(s => s.Parameters = @params.SingleOrDefault(p => p.SubscriptionId == s.Id)?.Parameters);
        }

        public async Task Delete(string subscriptionId, CancellationToken cancellationToken)
            => await _unitOfWork.Execute(PgSqlSubscriptionRequests.Delete, new { Id = subscriptionId }, cancellationToken);

        public async Task<IEnumerable<Guid>> GetPendingRequestIds(string subscriptionId, CancellationToken cancellationToken) 
            => await _unitOfWork.Query<Guid>(PgSqlSubscriptionRequests.ListPendingRequestIds, new { SubscriptionId = subscriptionId }, cancellationToken);

        public async Task AcknowledgePendingRequests(string subscriptionId, IEnumerable<Guid> requestIds, CancellationToken cancellationToken) 
            => await _unitOfWork.Execute(PgSqlSubscriptionRequests.AcknowledgePendingRequests, new { SubscriptionId = subscriptionId, RequestId = requestIds }, cancellationToken);

        public async Task RegisterSubscriptionTrigger(string subscriptionId, SubscriptionResult subscriptionResult, string reason, CancellationToken cancellationToken)
            => await _unitOfWork.Execute(PgSqlSubscriptionRequests.StoreTrigger, new { Id = Guid.NewGuid(), subscriptionId, Status = subscriptionResult, reason }, cancellationToken);

        public async Task Store(Subscription subscription, CancellationToken cancellationToken)
        {
            var entity = new
            {
                Active = true,
                subscription.SubscriptionId,
                subscription.Trigger,
                subscription.InitialRecordTime,
                subscription.ReportIfEmpty,
                subscription.Schedule?.Second,
                subscription.Schedule?.Hour,
                subscription.Schedule?.Minute,
                subscription.Schedule?.Month,
                subscription.Schedule?.DayOfMonth,
                subscription.Schedule?.DayOfWeek,
                subscription.Destination,
                subscription.QueryName
            };

            var entityId = await _unitOfWork.Store(PgSqlSubscriptionRequests.Store, entity, cancellationToken);

            var parameters = subscription.Parameters.Select(parameter => new { SubscriptionId = entityId, parameter.Name }).ToArray();
            var parameterIds = await _unitOfWork.Store(PgSqlSubscriptionRequests.StoreParameter, parameters, cancellationToken);
            var values = subscription.Parameters.SelectMany((p, i) => p.Values.Select(value => new { ParameterId = parameterIds[i], Value = value })).ToArray();

            await _unitOfWork.BulkExecute(PgSqlSubscriptionRequests.StoreParameterValue, values, cancellationToken);
        }
    }
}
