using System.Collections.Generic;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model
{
    public class CaptureCallbackRequest
    {
        public string SubscriptionId { get; set; }
        public QueryCallbackType CallbackType { get; set; }
        public EpcisRequestHeader Header { get; set; }
        public IList<EpcisEvent> EventList { get; set; }
    }
}
