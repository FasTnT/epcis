using FasTnT.Domain.Commands;

namespace FasTnT.Commands.Requests
{
    public class GetSubscriptionIdsRequest : IQueryRequest
    {
        public string QueryName { get; set; }
    }
}
