using FasTnT.Model.Events.Enums;
using System;

namespace FasTnT.Model
{
    public class EpcisQueryCallbackException : Request
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public QueryCallbackType CallbackType { get; set; }
        public string Severity { get; set; } = "ERROR";
        public string Reason { get; set; }
        public string SubscriptionName { get; set; }
    }
}
