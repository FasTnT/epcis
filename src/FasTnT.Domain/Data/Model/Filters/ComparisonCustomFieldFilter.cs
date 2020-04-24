using FasTnT.Model.Enums;
using FasTnT.Model.Events;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ComparisonCustomFieldFilter
    {
        public CustomField Field { get; set; }
        public object Value { get; set; }
        public bool IsInner { get; set; }
        public FilterComparator Comparator { get; set; }
    }
}
