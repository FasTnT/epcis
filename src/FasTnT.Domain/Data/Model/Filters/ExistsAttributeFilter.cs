using FasTnT.Model.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class ExistsAttributeFilter
    {
        public EpcisField Field { get; set; }
        public string AttributeName { get; set; }
    }
}
