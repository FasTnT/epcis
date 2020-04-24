using FasTnT.Model;
using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model
{
    public class CaptureCallbackRequest : EpcisRequest
    {
        public string SubscriptionId { get; set; }
        public QueryCallbackType CallbackType { get; set; }
        public EpcisRequest Header { get; set; }
    }
}
