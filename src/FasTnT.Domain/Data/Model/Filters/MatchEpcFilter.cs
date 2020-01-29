using FasTnT.Model.Events.Enums;

namespace FasTnT.Domain.Data.Model.Filters
{
    public class MatchEpcFilter
    {
        public EpcType[] EpcType { get; set; }
        public object[] Values { get; set; }
    }
}
