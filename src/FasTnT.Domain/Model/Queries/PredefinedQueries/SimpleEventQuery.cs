using FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters;
using System.Collections.Generic;

namespace FasTnT.Domain.Model.Queries.Implementations.PredefinedQueries
{
    public class SimpleEventQuery : PredefinedQuery
    {
        public const string Name = "SimpleEventQuery";

        public override bool AllowsSubscription => true;
        public IEnumerable<SimpleEventQueryParameter> Parameters { get; set; }
    }
}
