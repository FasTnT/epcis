using FasTnT.Model;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ExistCustomFieldFilter
    {
        public CustomField Field { get; set; }
        public bool IsInner { get; set; }
    }

    public class ExistsAttributeFilter
    {
        public EpcisField Field { get; set; }
        public string AttributeName { get; set; }
    }

    public class AttributeFilter
    {
        public EpcisField Field { get; set; }
        public string AttributeName { get; set; }
        public string[] Values { get; set; }
    }
}
