using FasTnT.Model;
using FasTnT.Model.Events.Enums;

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
