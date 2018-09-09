using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public class CaptureSucceedResponse : IEpcisResponse
    {
        public void Accept(IEpcisResponseFormatter formatter)
        {
            formatter.Accept(this);
        }
    }
}
