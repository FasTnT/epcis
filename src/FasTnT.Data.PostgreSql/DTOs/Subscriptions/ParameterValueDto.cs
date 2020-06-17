namespace FasTnT.Data.PostgreSql.DTOs.Subscriptions
{
    public class ParameterValueDto
    {
        public int SubscriptionId { get; set; }
        public short ParameterId { get; set; }
        public string Value { get; set; }

        internal static ParameterValueDto Create(string value, short paramId, int subscriptionId)
        {
            return new ParameterValueDto
            {
                SubscriptionId = subscriptionId,
                ParameterId = paramId,
                Value = value
            };
        }
    }
}
