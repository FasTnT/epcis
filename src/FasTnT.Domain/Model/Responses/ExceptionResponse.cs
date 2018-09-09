using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public class ExceptionResponse : IEpcisResponse
    {
        public string Exception { get; set; }
        public string Reason { get; set; }

        public void Accept(IEpcisResponseFormatter formatter) => formatter.Accept(this);
    }
}
