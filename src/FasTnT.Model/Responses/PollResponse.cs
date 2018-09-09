using System.Collections.Generic;

namespace FasTnT.Model.Responses
{
    public class PollResponse : IEpcisResponse
    {
        public string QueryName { get; set; }
        public string SubscriptionId { get; set; }
        public IEnumerable<IEntity> Entities { get; set; }
    }
}
