using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;

namespace FasTnT.Formatters.Json
{
    public static class ODataQueryParameterParser
    {
        public static IEnumerable<QueryParameter> ParseODataQueryString(string queryString)
        {
            var parameters = new List<QueryParameter>();
            var queryParts = queryString.TrimStart('?').Split('&');

            // TODO: parse QueryString

            return parameters;
        }
    }
}
