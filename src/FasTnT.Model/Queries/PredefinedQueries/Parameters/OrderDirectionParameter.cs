using FasTnT.Model.Queries.PredefinedQueries.Parameters.Enums;
using FasTnT.Model.Utils;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class OrderDirectionParameter : SimpleEventQueryParameter
    {
        public OrderDirection Direction => Enumeration.GetByDisplayName<OrderDirection>(Value);
    }
}
