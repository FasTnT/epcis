using FasTnT.Model.Exceptions;

namespace FasTnT.Commands.Responses
{
    public class ExceptionResponse : IEpcisResponse
    {
        public string Exception { get; set; }
        public ExceptionSeverity Severity { get; set; }
        public string Reason { get; set; }
    }
}
