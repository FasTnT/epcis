using FasTnT.Domain;
using FasTnT.Model.Utils;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class ActionParameter : SimpleEventQueryParameter
    {
        public IEnumerable<EventAction> Actions => Values.Select(Enumeration.GetByDisplayName<EventAction>);
    }
}
