using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Data.PostgreSql.Query
{
    public class QueryParameters
    {
        public IDictionary<string, object> Values { get; set; } = new SortedDictionary<string, object>();

        public string Last => $"@qp_{Values.Keys.Count - 1}";
        public string Add<T>(T value)
        {
            return Values.TryAdd($"qp_{Values.Keys.Count}", GetSqlValue(value)) 
                ? Last 
                : throw new Exception("Impossible to create SQL parameter.");
        }

        private static object GetSqlValue<T>(T value)
        {
            return value switch
            {
                IEnumerable<Enumeration> enumList => enumList.Select(x => x.Id).ToArray(),
                IEnumerable<object> objList => objList.Select(x => x.ToString()).ToArray(),
                Enumeration enumOut => enumOut.Id,
                _ => value
            };
        }

        public void SetLimit(int value) => Values.Add("limit", value);
    }
}
