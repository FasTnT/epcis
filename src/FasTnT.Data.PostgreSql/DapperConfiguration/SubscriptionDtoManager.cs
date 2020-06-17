using Dapper;
using FasTnT.Data.PostgreSql.DTOs.Subscriptions;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Model.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public class SubscriptionDtoManager
    {
        public List<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
        public List<ParameterDto> Parameters { get; set; } = new List<ParameterDto>();
        public List<ParameterValueDto> ParameterValues { get; set; } = new List<ParameterValueDto>();

        internal static async Task<SubscriptionDtoManager> ReadAsync(SqlMapper.GridReader reader)
        {
            var manager = new SubscriptionDtoManager();

            manager.Subscriptions.AddRange(await reader.ReadAsync<SubscriptionDto>());
            manager.Parameters.AddRange(await reader.ReadAsync<ParameterDto>());
            manager.ParameterValues.AddRange(await reader.ReadAsync<ParameterValueDto>());

            return manager;
        }

        internal IEnumerable<Subscription> FormatSubscriptions()
        {
            return Subscriptions.Select(sub =>
            {
                var subscription = sub.ToSubscription();
                subscription.Parameters.AddRange(FormatParameters(sub.Id));

                return subscription;
            });
        }

        private IEnumerable<QueryParameter> FormatParameters(int id)
        {
            return Parameters.Where(x => x.SubscriptionId == id).Select(p =>
            {
                var parameter = p.ToParameter();
                var values = ParameterValues.Where(x => x.ParameterId == p.Id && x.SubscriptionId == p.SubscriptionId);

                parameter.Values = values.Select(x => x.Value).ToArray();

                return parameter;
            });
        }
    }
}
