using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class OrderDirection : Enumeration
    {
        public static readonly OrderDirection Ascending = new OrderDirection(0, "ASC");
        public static readonly OrderDirection Descending = new OrderDirection(1, "DESC");

        public OrderDirection() { }
        public OrderDirection(short id, string displayName) : base(id, displayName) { }
    }
}
