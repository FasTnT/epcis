using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class SourceDestinationFilter
    {
        public string Name { get; set; }
        public SourceDestinationType Type { get; set; }
        public object[] Values { get; set; }
    }
}
