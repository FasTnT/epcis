using FasTnT.Model.Utils;

namespace FasTnT.Model.Exceptions
{
    public class ExceptionType : Enumeration
    {
        public static ExceptionType SubscribeNotPermittedException = new ExceptionType(0, nameof(SubscribeNotPermittedException));
        public static ExceptionType ImplementationException = new ExceptionType(1, nameof(ImplementationException));
        public static ExceptionType NoSuchNameException = new ExceptionType(2, nameof(NoSuchNameException));
        public static ExceptionType QueryTooLargeException = new ExceptionType(3, nameof(QueryTooLargeException));
        public static ExceptionType QueryParameterException = new ExceptionType(4, nameof(QueryParameterException));
        public static ExceptionType ValidationException = new ExceptionType(5, nameof(ValidationException));
        public static ExceptionType SubscriptionControlsException = new ExceptionType(6, nameof(SubscriptionControlsException));
        public static ExceptionType NoSuchSubscriptionException = new ExceptionType(7, nameof(NoSuchSubscriptionException));
        public static ExceptionType DuplicateSubscriptionException = new ExceptionType(8, nameof(DuplicateSubscriptionException));
        public static ExceptionType QueryTooComplexException = new ExceptionType(9, nameof(QueryTooComplexException));
        public static ExceptionType InvalidURIException = new ExceptionType(10, nameof(InvalidURIException));

        public ExceptionType() { }
        public ExceptionType(short id, string displayName) : base(id, displayName) { }
    }
}
