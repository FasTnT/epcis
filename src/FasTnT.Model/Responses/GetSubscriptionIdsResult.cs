using System.Collections.Generic;

namespace FasTnT.Model.Responses
{
    public class GetSubscriptionIdsResult : IEpcisResponse
    {
        public IEnumerable<string> SubscriptionIds { get; set; }
    }
}
