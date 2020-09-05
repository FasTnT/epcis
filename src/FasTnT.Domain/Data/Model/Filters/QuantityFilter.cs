using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class QuantityFilter
    {
        public FilterComparator Operator { get; set; }
        public double Value { get; set; }
    }
}
