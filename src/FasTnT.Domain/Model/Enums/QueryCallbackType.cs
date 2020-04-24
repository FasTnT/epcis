using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class QueryCallbackType : Enumeration
    {
        public static QueryCallbackType Success = new QueryCallbackType(0, "success");
        public static QueryCallbackType QueryTooLargeException = new QueryCallbackType(1, "queryTooLargeException");
        public static QueryCallbackType ImplementationException = new QueryCallbackType(2, "implementationException");

        public QueryCallbackType() {}
        public QueryCallbackType(short id, string displayName) : base(id, displayName) {}
    }
}
