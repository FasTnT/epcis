using FasTnT.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Binders
{
    public class EpcisModelBinder<T> : IModelBinder
    {
        private readonly IFormatter<T> _xmlFormatter;
        private readonly IFormatter<T> _jsonFormatter;

        public EpcisModelBinder(IFormatter<T> xmlFormatter, IFormatter<T> jsonFormatter)
        {
            _xmlFormatter = xmlFormatter;
            _jsonFormatter = jsonFormatter;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var parser = GetParserFromContext(bindingContext.HttpContext);
            var inputStream = bindingContext.ActionContext?.HttpContext?.Request?.Body;

            if (inputStream == null || !inputStream.CanRead) throw new Exception("HTTP context is null or body can't be read");

            bindingContext.Result = ModelBindingResult.Success(parser.Read(inputStream));

            return Task.CompletedTask;
        }

        private IFormatter<T> GetParserFromContext(HttpContext httpContext)
        {
            switch (httpContext.Request.ContentType.ToLower())
            {
                case "application/xml":
                case "text/xml":
                    return _xmlFormatter;
                case "application/json":
                    return _jsonFormatter;
                default:
                    throw new Exception($"Content-Type '{httpContext.Request.ContentType}' is not supported.");
            }
        }
    }
}
