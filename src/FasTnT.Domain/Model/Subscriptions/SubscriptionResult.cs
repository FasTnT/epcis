using FasTnT.Model.Utils;

namespace FasTnT.Domain.Model.Subscriptions
{
    public class SubscriptionResult : Enumeration
    {
        public static SubscriptionResult Success = new SubscriptionResult(0, nameof(Success));
        public static SubscriptionResult Failed = new SubscriptionResult(1, nameof(Failed));

        public SubscriptionResult() { }
        public SubscriptionResult(short id, string displayName) : base(id, displayName) { }
    }
}
