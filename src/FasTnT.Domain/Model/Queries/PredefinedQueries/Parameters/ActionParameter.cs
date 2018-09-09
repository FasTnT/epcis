using FasTnT.Domain.Utils;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters
{
    public class ActionParameter : SimpleEventQueryParameter
    {
        public IEnumerable<EventAction> Actions => Values.Select(Enumeration.GetByDisplayName<EventAction>);
    }
}
