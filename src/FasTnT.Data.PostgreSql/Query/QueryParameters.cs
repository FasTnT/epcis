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

        private object GetSqlValue<T>(T value)
        {
            switch (value)
            {
                case IEnumerable<Enumeration> enumList:
                    return enumList.Select(x => x.Id).ToArray();
                case IEnumerable<object> objList:
                    return objList.ToArray();
                case Enumeration enumOut:
                    return enumOut.Id;
                default:
                    return value;
            }
        }

        public void SetLimit(int value) => Values.Add("limit", value);
    }
}
