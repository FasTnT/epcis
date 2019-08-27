using FasTnT.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Formatters.Json
{
    public static class QueryStringParameterParser
    {
        public static IEnumerable<QueryParameter> ParseODataQueryString(string queryString)
        {
            var queryParts = (queryString ?? string.Empty).TrimStart('?').Split('&').Where(p => p.Contains('=')).ToArray();
            var parameters = new QueryParameter[queryParts.Length];

            for(var i=0; i<queryParts.Length; i++)
            {
                parameters[i] = new QueryParameter
                {
                    Name = queryParts[i].Split('=')[0],
                    Values = queryParts[i].Split('=')[1].Split(';').ToArray(),
                };
            }

            return parameters;
        }
    }
}
