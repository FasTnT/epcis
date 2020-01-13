using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public class QueryParameters
    {
        public IDictionary<string, object> Values { get; set; } = new SortedDictionary<string, object>();

        public string Last => $"@qp_{Values.Keys.Count - 1}";
        public string Add<T>(IEnumerable<T> value) => Add(value.ToArray());

        public string Add<T>(T value) => Values.TryAdd($"qp_{Values.Keys.Count}", value) ? Last : throw new Exception("Impossible to create SQL parameter.");
        public void SetLimit(int value) => Values.Add("limit", value);
    }
}
