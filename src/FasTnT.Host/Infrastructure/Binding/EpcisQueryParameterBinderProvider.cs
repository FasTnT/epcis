using FasTnT.Formatters.Json;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisQueryParameterBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return context.Metadata.ModelType == typeof(IEnumerable<QueryParameter>)
                ? new EpcisQueryParameterModelBinder()
                : null;
        }

        private class EpcisQueryParameterModelBinder : IModelBinder
        {
            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var httpCtx = bindingContext.HttpContext;
                var queryString = httpCtx.Request.QueryString;
                var parameters = QueryStringParameterParser.ParseQueryString(queryString.HasValue ? queryString.Value : default);

                await Task.Run(() => bindingContext.Result = ModelBindingResult.Success(parameters));
            }
        }
    }
}
