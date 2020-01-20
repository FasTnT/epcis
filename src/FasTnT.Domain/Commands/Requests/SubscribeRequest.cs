using FasTnT.Domain.Commands;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Commands.Requests
{
    public class SubscribeRequest : IQueryRequest
    {
        public Subscription Subscription { get; set; }
    }
}
