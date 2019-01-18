using FasTnT.Model.Exceptions;

namespace FasTnT.Model.Responses
{
    public class ExceptionResponse : IEpcisResponse
    {
        public string Exception { get; set; }
        public string Reason { get; set; }
        public ExceptionSeverity Severity { get; set; }
    }
}
