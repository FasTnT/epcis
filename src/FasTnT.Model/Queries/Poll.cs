using System.Collections.Generic;

namespace FasTnT.Model.Queries
{
    public class Poll : EpcisQuery
    {
        public string QueryName { get; set; }
        public IEnumerable<QueryParameter> Parameters { get; set; } = new QueryParameter[0];
    }
}
