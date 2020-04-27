using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class AttributeFilter
    {
        public EpcisField Field { get; set; }
        public string AttributeName { get; set; }
        public string[] Values { get; set; }
    }
}
