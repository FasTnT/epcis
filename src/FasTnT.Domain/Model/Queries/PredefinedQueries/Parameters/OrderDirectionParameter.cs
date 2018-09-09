using FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters.Enums;
using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters
{
    public class OrderDirectionParameter : SimpleEventQueryParameter
    {
        public OrderDirection Direction => Enumeration.GetByDisplayName<OrderDirection>(Value);
    }
}
