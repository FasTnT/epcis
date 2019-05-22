using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FasTnT.Model.Queries;
using System.Linq;

namespace FasTnT.Host.Infrastructure
{
    public class QueryParameterInputFormatter : InputFormatter
    {
        public QueryParameterInputFormatter() => SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        protected override bool CanReadType(Type type) => type == typeof(IEnumerable<QueryParameter>);

        // TODO: parse OData query
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var queryString = context.HttpContext.Request.QueryString;
            var parameters = queryString.Value
                                        .TrimStart('?')
                                        .Split('&')
                                        .Where(x => x.Contains('='))
                                        .Select(x => new QueryParameter { Name = x.Split('=')[0], Values = x.Split('=')[1].Split(',') })
                                        .ToList();

            return await InputFormatterResult.SuccessAsync(parameters);
        }
    }
}