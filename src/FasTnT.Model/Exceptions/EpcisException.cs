using System;

namespace FasTnT.Model.Exceptions
{
    public class EpcisException : Exception
    {
        public ExceptionType ExceptionType { get; }
        public ExceptionSeverity Severity { get; }

        public EpcisException(ExceptionType exceptionType, string message, ExceptionSeverity severity = null) : base(message)
        {
            ExceptionType = exceptionType;
        }
    }
}
