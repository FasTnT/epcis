using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class QuantityFilter
    {
        public FilterComparator Operator { get; set; }
        public double Value { get; set; }
    }
}
