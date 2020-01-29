using FasTnT.Domain.Commands;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisExceptionRequest : ICaptureRequest
    {
        public EpcisRequestHeader Header { get; set; }
        public QueryCallbackType CallbackType { get; set; }
        public string Severity { get; set; } = "ERROR";
        public string Reason { get; set; }
        public string SubscriptionName { get; set; }
    }
}
