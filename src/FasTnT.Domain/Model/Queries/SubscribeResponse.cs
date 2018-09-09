using FasTnT.Domain.Formatter;
using FasTnT.Domain.Model.Responses;

namespace FasTnT.Domain.Model.Queries
{
    public class SubscribeResponse : IEpcisResponse
    {
        public void Accept(IEpcisResponseFormatter formatter) => formatter.Accept(this);
    }
}
