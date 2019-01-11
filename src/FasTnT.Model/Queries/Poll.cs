using System.Collections.Generic;

namespace FasTnT.Model.Queries
{
    public class Poll : EpcisQuery
    {
        public string QueryName { get; set; }
        public IEnumerable<QueryParameter> Parameters { get; set; }
    }

    public class QueryParameter
    {
        public string Name { get; set; }
        public string[] Values { get; set; }
    }
}
