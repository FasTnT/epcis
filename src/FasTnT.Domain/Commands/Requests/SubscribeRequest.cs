using FasTnT.Domain.Commands;

namespace FasTnT.Commands.Requests
{
    public class SubscribeRequest : IQueryRequest
    {
        public string QueryName { get; set; }
    }
}
