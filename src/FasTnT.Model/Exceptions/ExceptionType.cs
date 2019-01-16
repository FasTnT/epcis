using FasTnT.Model.Utils;

namespace FasTnT.Model.Exceptions
{
    public class ExceptionType : Enumeration
    {
        public static ExceptionType SubscribeNotPermittedException = new ExceptionType(0, "SubscribeNotPermittedException");
        public static ExceptionType ImplementationException = new ExceptionType(1, "ImplementationException");
        public static ExceptionType NoSuchNameException = new ExceptionType(2, "NoSuchNameException");
        public static ExceptionType QueryTooLargeException = new ExceptionType(3, "QueryTooLargeException");
        public static ExceptionType QueryParameterException = new ExceptionType(4, "QueryParameterException");
        public static ExceptionType ValidationException = new ExceptionType(5, "ValidationException");

        public ExceptionType() { }
        public ExceptionType(short id, string displayName) : base(id, displayName) { }
    }
}
