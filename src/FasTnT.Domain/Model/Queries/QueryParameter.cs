using System.Collections.Generic;

namespace FasTnT.Domain.Model.Queries
{
    public abstract class QueryParameter
    {
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string Value { get { return Values?.SingleWhenOnly();  } }
    }
}
