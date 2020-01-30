using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class SimpleParameterFilter<T>
    {
        public EpcisField Field { get; set; }
        public T[] Values { get; set; }
    }
}
