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
        private readonly IUnitOfWork _unitOfWork;

        public PgSqlSubscriptionManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Subscription>> GetAll(bool withDetails = false)
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
            });

            if (withDetails)
            {
                var queryParameters = default(IEnumerable<QueryParameter>);

                foreach (var subscription in subscriptions)
                {
                    subscription.Parameters = queryParameters;
                }
            }

            return subscriptions;
        }

        public async Task<IEnumerable<Subscription>> ListForQuery(string queryName) 
            => await _unitOfWork.Query<Subscription>(SqlRequests.SubscriptionListIds, new { QueryName = queryName });

        public async Task Delete(Guid id)
            => await _unitOfWork.Execute(SqlRequests.SubscriptionDelete, new { Id = id });

        public async Task<IEnumerable<Guid>> GetPendingRequestIds(Guid subscriptionId) 
            => await _unitOfWork.Query<Guid>(SqlRequests.SubscriptionListPendingRequestIds, new { SubscriptionId = subscriptionId });

        public async Task AcknowledgePendingRequests(Guid subscriptionId, IEnumerable<Guid> requestIds) 
            => await _unitOfWork.Execute(SqlRequests.SubscriptionAcknowledgePendingRequests, new { SubscriptionId = subscriptionId, RequestId = requestIds });

        public async Task Store(Subscription subscription)
        {
            var parameters = new List<object>();
            var parameterValues = new List<object>();

            subscription.Parameters.ForEach(parameter =>
            {
                var id = Guid.NewGuid();

                parameters.Add(new
                {
                    Id = id,
                    SubscriptionId = subscription.Id,
                    parameter.Name
                });

                parameter.Values.ForEach(value =>
                    parameterValues.Add(new
                    {
                        Id = Guid.NewGuid(),
                        ParameterId = id,
                        Value = value
                    }));
            });

            using (new CommitOnDisposeScope(_unitOfWork))
            {
                await _unitOfWork.Execute(SqlRequests.SubscriptionStore, GetPgSqlSubscription(subscription));
                await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameter, parameters);
                await _unitOfWork.Execute(SqlRequests.SubscriptionStoreParameterValue, parameterValues);
            }
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
