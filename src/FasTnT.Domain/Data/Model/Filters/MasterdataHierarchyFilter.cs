using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class MasterdataHierarchyFilter
    {
        public EpcisField Field { get; set; }
        public object[] Values { get; set; }
    }
}
