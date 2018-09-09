using System.Collections.Generic;
using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public class PollResponse : IEpcisResponse
    {
        public string QueryName { get; set; }
        public string SubscriptionId { get; set; }
        public IEnumerable<IEntity> Entities { get; set; }

        public void Accept(IEpcisResponseFormatter formatter) => formatter.Accept(this);
    }
}
