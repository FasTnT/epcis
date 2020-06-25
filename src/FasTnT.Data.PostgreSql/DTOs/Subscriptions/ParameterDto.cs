using FasTnT.Model.Queries;

namespace FasTnT.Data.PostgreSql.DTOs.Subscriptions
{
    public class ParameterDto
    {
        public int SubscriptionId { get; set; }
        public short Id { get; set; }
        public string Name { get; set; }

        internal static ParameterDto Create(QueryParameter parameter, short id, int subscriptionId)
        {
            return new ParameterDto
            {
                SubscriptionId = subscriptionId,
                Id = id,
                Name = parameter.Name
            };
        }

        internal QueryParameter ToParameter()
        {
            return new QueryParameter
            {
                Name = Name
            };
        }
    }
}
