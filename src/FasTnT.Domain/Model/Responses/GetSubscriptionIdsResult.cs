using System.Collections.Generic;
using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public class GetSubscriptionIdsResult : IEpcisResponse
    {
        public IEnumerable<string> SubscriptionIds { get; set; }

        public void Accept(IEpcisResponseFormatter formatter) => formatter.Accept(this);
    }
}
