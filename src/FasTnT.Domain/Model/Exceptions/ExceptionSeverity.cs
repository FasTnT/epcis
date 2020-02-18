using FasTnT.Model.Utils;

namespace FasTnT.Model.Exceptions
{
    public class ExceptionSeverity : Enumeration
    {
        public static ExceptionSeverity Error = new ExceptionSeverity(4, "ERROR");
        public static ExceptionSeverity Severe = new ExceptionSeverity(5, "SEVERE");

        public ExceptionSeverity() { }
        public ExceptionSeverity(short id, string displayName) : base(id, displayName) { }
    }
}
