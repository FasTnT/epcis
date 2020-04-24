using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ComparisonParameterFilter
    {
        public EpcisField Field { get; set; }
        public FilterComparator Comparator { get; set; }
        public object Value { get; set; }
    }
}
