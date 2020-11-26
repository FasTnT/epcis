using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class FilterComparator : Enumeration
    {
        public static readonly FilterComparator GreaterThan = new FilterComparator(0, "GT");
        public static readonly FilterComparator GreaterOrEqual = new FilterComparator(1, "GE");
        public static readonly FilterComparator LessThan = new FilterComparator(2, "LT");
        public static readonly FilterComparator LessOrEqual = new FilterComparator(3, "LE");
        public static readonly FilterComparator Equal = new FilterComparator(4, "EQ");

        public FilterComparator() { }
        public FilterComparator(short id, string displayName) : base(id, displayName) { }
    }
}
