using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Subscription> GetById(string subscriptionId)
        {
            return (await _unitOfWork.Query<Subscription>($"{SqlRequests.SubscriptionListIds} WHERE s.subscription_id = @Id", new { Id = subscriptionId })).SingleOrDefault();
        }

        public async Task<IEnumerable<Subscription>> GetAll(bool includeDetails = false)
        {
            var subscriptions = (await _unitOfWork.Query<dynamic>(SqlRequests.SubscriptionsList)).Select(x => new Subscription
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

            if (includeDetails) await LoadParameters(subscriptions);

            return subscriptions;
        }

        private async Task LoadParameters(Subscription[] subscriptions)
        {
            var @params = (await _unitOfWork.Query<dynamic>(SqlRequests.SubscriptionListParameters))
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

        public async Task Delete(Guid id)
            => await _unitOfWork.Execute(SqlRequests.SubscriptionDelete, new { Id = id });

        public async Task<IEnumerable<Guid>> GetPendingRequestIds(Guid subscriptionId) 
            => await _unitOfWork.Query<Guid>(SqlRequests.SubscriptionListPendingRequestIds, new { SubscriptionId = subscriptionId });

        public async Task AcknowledgePendingRequests(Guid subscriptionId, IEnumerable<Guid> requestIds) 
            => await _unitOfWork.Execute(SqlRequests.SubscriptionAcknowledgePendingRequests, new { SubscriptionId = subscriptionId, RequestId = requestIds });

        public async Task Store(Subscription subscription)
        {
            var parameters = new List<object>();
            var values = new List<object>();

            subscription.Parameters.ForEach(parameter =>
            {
                var id = Guid.NewGuid();

                parameters.Add(new { Id = id, SubscriptionId = subscription.Id, parameter.Name });
                parameter.Values.ForEach(value => values.Add(new { Id = Guid.NewGuid(), ParameterId = id, Value = value }));
            });

            await _unitOfWork.Execute(SqlRequests.SubscriptionStore, GetPgSqlSubscription(subscription));
            await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameter, parameters);
            await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameterValue, values);
        }

        private object GetPgSqlSubscription(Subscription subscription)
        {
            return new
            {
                subscription.Id,
                subscription.SubscriptionId,
                subscription.QueryName,
                subscription.ReportIfEmpty,
                subscription.Trigger,
                subscription.InitialRecordTime,
                subscription.Destination,
                subscription.Active,
                subscription.Schedule.Second,
                subscription.Schedule.Minute,
                subscription.Schedule.Hour,
                subscription.Schedule.Month,
                subscription.Schedule.DayOfMonth,
                subscription.Schedule.DayOfWeek
            };
        }
    }
}
