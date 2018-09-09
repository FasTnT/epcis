using FasTnT.Model.Utils;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class ParameterComparator : Enumeration
    {
        public static ParameterComparator Equal = new ParameterComparator(0, "EQ");
        public static ParameterComparator GreaterThan = new ParameterComparator(1, "GT");
        public static ParameterComparator GreaterThanOrEqual = new ParameterComparator(2, "GE");
        public static ParameterComparator LessThan = new ParameterComparator(3, "LT");
        public static ParameterComparator LessThanOrEqual = new ParameterComparator(4, "LE");

        public ParameterComparator() { }
        public ParameterComparator(short id, string displayName) : base(id, displayName) { }      
    }
}
