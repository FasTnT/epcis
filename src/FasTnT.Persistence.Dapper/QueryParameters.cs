using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Persistence.Dapper
{
    public class QueryParameters
    {
        public IDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        public string Last => $"@qp_{Values.Keys.Count-1}";
        public string Add<T>(IEnumerable<T> value) => Add(value.ToArray());

        public string Add<T>(T value)
        {
            var name = $"qp_{Values.Keys.Count}";
            return Values.TryAdd(name, value) ? $"@{name}" : throw new Exception("Impossible to create SQL parameter.");
        }

        public void SetLimit(int value) =>  Values.Add("limit", value);
    }
}
