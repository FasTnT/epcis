using System;

namespace FasTnT.Model.Exceptions
{
    public class EpcisException : Exception
    {
        public ExceptionType ExceptionType { get; }

        public EpcisException(ExceptionType exceptionType, string message) : base(message)
        {
            ExceptionType = exceptionType;
        }
    }
}
