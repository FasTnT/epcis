using System.Collections.Generic;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public abstract class SimpleEventQueryParameter {
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string Value { get { return Values?.SingleWhenOnly(); } }
    }
}
