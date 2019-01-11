using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class OrderDirection : Enumeration
    {
        public static OrderDirection Ascending = new OrderDirection(0, "ASC");
        public static OrderDirection Descending = new OrderDirection(1, "DESC");

        public OrderDirection()
        {
        }

        public OrderDirection(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
