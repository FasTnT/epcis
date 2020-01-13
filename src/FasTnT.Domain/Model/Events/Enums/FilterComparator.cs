using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class FilterComparator : Enumeration
    {
        public static FilterComparator GreaterThan = new FilterComparator(0, "GT");
        public static FilterComparator GreaterOrEqual = new FilterComparator(1, "GE");
        public static FilterComparator LessThan = new FilterComparator(2, "LT");
        public static FilterComparator LessOrEqual = new FilterComparator(3, "LE");
        public static FilterComparator Equal = new FilterComparator(4, "EQ");

        public FilterComparator() { }
        public FilterComparator(short id, string displayName) : base(id, displayName) { }
    }
}
