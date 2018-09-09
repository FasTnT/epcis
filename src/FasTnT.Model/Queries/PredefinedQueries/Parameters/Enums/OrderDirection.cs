using FasTnT.Model.Utils;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters.Enums
{
    public class OrderDirection : Enumeration
    {
        public static OrderDirection Asc = new OrderDirection(0, "ASC");
        public static OrderDirection Desc = new OrderDirection(1, "DESC");

        public OrderDirection()
        {
        }

        public OrderDirection(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
