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
            return (await _unitOfWork.Query<Subscription>($"{SqlRequests.SubscriptionListIds} WHERE s.subscription_id = @Id", new { Id = subscriptionId }, cancellationToken)).SingleOrDefault();
        }

        public async Task<IEnumerable<Subscription>> GetAll(bool includeDetails, CancellationToken cancellationToken)
        {
            var subscriptions = (await _unitOfWork.Query<dynamic>(SqlRequests.SubscriptionsList, null, cancellationToken)).Select(x => new SubscriptionEntity
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
            var @params = (await _unitOfWork.Query<dynamic>(SqlRequests.SubscriptionListParameters, null, cancellationToken))
                .GroupBy(x => (Guid)x.subscription_id)
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
            => await _unitOfWork.Execute(SqlRequests.SubscriptionDelete, new { Id = subscriptionId }, cancellationToken);

        public async Task<IEnumerable<Guid>> GetPendingRequestIds(string subscriptionId, CancellationToken cancellationToken) 
            => await _unitOfWork.Query<Guid>(SqlRequests.SubscriptionListPendingRequestIds, new { SubscriptionId = subscriptionId }, cancellationToken);

        public async Task AcknowledgePendingRequests(string subscriptionId, IEnumerable<Guid> requestIds, CancellationToken cancellationToken) 
            => await _unitOfWork.Execute(SqlRequests.SubscriptionAcknowledgePendingRequests, new { SubscriptionId = subscriptionId, RequestId = requestIds }, cancellationToken);

        public async Task Store(Subscription subscription, CancellationToken cancellationToken)
        {
            var entity = new
            {
                Id = Guid.NewGuid(),
                Active = true,
                subscription.SubscriptionId,
                subscription.Trigger,
                subscription.InitialRecordTime,
                subscription.ReportIfEmpty,
                subscription.Schedule.Second,
                subscription.Schedule.Hour,
                subscription.Schedule.Minute,
                subscription.Schedule.Month,
                subscription.Schedule.DayOfMonth,
                subscription.Schedule.DayOfWeek,
                subscription.Destination,
                subscription.QueryName
            };
            var parameters = new List<object>();
            var values = new List<object>();

            subscription.Parameters.ForEach(parameter =>
            {
                var id = Guid.NewGuid();

                parameters.Add(new { Id = id, SubscriptionId = entity.Id, parameter.Name });
                parameter.Values.ForEach(value => values.Add(new { Id = Guid.NewGuid(), ParameterId = id, Value = value }));
            });

            await _unitOfWork.Execute(SqlRequests.SubscriptionStore, entity, cancellationToken);
            await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameter, parameters, cancellationToken);
            await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameterValue, values, cancellationToken);
        }
    }
}
