using FasTnT.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FasTnT.Host.Infrastructure.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Binders
{
    public class EpcisModelBinder<T> : IModelBinder
    {
        private readonly IFormatter<T> _xmlFormatter;
        private readonly IFormatter<T> _jsonFormatter;
        private readonly ILogger _logger;

        public EpcisModelBinder(IFormatter<T> xmlFormatter, IFormatter<T> jsonFormatter)
        {
            _xmlFormatter = xmlFormatter;
            _jsonFormatter = jsonFormatter;
            _logger = ApplicationLogging.CreateLogger<EpcisModelBinder<T>>();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var parser = GetParserFromContext(bindingContext.HttpContext);
            var inputStream = bindingContext.ActionContext?.HttpContext?.Request?.Body;

            if (inputStream == null || !inputStream.CanRead) throw new Exception("HTTP context is null or body can't be read");

            try
            {
                bindingContext.Result = ModelBindingResult.Success(parser.Read(inputStream));
            }
            catch
            {
                _logger.LogError($"[{bindingContext.HttpContext.TraceIdentifier}] Failed to parse {typeof(T).Name} from {bindingContext.HttpContext.Request.ContentType} request body");
                throw;
            }

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
