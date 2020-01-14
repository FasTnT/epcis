using FasTnT.Model;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class SimpleParameterFilter
    {
        public EpcisField Field { get; set; }
        public object[] Values { get; set; }
    }

    public class ComparisonParameterFilter
    {
        public EpcisField Field { get; set; }
        public FilterComparator Comparator { get; set; }
        public object Value { get; set; }
    }

    public class BusinessTransactionFilter
    {
        public string TransactionType { get; set; }
        public object[] Values { get; set; }
    }

    public class MatchEpcFilter
    {
        public EpcType[] EpcType { get; set; }
        public object[] Values { get; set; }
    }

    public class QuantityFilter
    {
        public FilterComparator Operator { get; set; }
        public double Value { get; set; }
    }

    public class CustomFieldFilter
    {
        public CustomField Field { get; set; }
        public object[] Values { get; set; }
        public bool IsInner { get; set; }
        public FilterComparator Comparator { get; set; }
    }
    public class ExistCustomFieldFilter
    {
        public CustomField Field { get; set; }
        public bool IsInner { get; set; }
    }

    public class LimitFilter
    {
        public int Value { get; set; }
    }
}
