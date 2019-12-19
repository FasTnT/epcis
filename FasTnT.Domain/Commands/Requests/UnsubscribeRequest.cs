using FasTnT.Domain.Commands;

namespace FasTnT.Commands.Requests
{
    public class UnsubscribeRequest : IQueryRequest
    {
        public string SubscriptionId { get; set; }
    }
}
