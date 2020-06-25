using FasTnT.Model;

namespace FasTnT.Data.PostgreSql.DTOs
{
    internal class SubscriptionCallbackDto
    {
        public int HeaderId { get; set; }
        public string SubscriptionId { get; set; }
        public short CallbackType { get; set; }
        public string Reason { get; set; }

        internal static SubscriptionCallbackDto Create(SubscriptionCallback callback, int headerId)
        {
            return new SubscriptionCallbackDto
            {
                HeaderId = headerId,
                SubscriptionId = callback.SubscriptionId,
                CallbackType = callback.CallbackType.Id,
                Reason = callback.Reason
            };
        }
    }
}
