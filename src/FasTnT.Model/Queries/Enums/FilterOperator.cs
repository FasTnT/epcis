using FasTnT.Model.Utils;

namespace FasTnT.Model.Queries.Enums
{
    public class FilterOperator : Enumeration
    {
        public static FilterOperator GreaterThan = new FilterOperator(0, "GT");
        public static FilterOperator GreaterOrEqual = new FilterOperator(1, "GE");
        public static FilterOperator LessThan = new FilterOperator(2, "LT");
        public static FilterOperator LessOrEqual = new FilterOperator(3, "LE");
        public static FilterOperator Equal = new FilterOperator(4,  "EQ");

        public FilterOperator()
        {
        }

        public FilterOperator(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
