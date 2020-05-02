using FasTnT.Model.Enums;

namespace FasTnT.Model
{
    public class SubscriptionCallback
    {
        public string SubscriptionId { get; set; }
        public QueryCallbackType CallbackType { get; set; }
        public string Reason { get; set; }
    }
}
