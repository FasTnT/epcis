using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class QueryCallbackType : Enumeration
    {
        public static readonly QueryCallbackType Success = new QueryCallbackType(0, "queryResults");
        public static readonly QueryCallbackType QueryTooLargeException = new QueryCallbackType(1, "queryTooLargeException");
        public static readonly QueryCallbackType ImplementationException = new QueryCallbackType(2, "implementationException");

        public QueryCallbackType() {}
        public QueryCallbackType(short id, string displayName) : base(id, displayName) {}
    }
}
