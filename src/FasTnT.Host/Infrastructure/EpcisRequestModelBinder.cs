using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Binders
{
    public class EpcisRequestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var parser = GetParserFromContext(bindingContext.HttpContext);
            var inputStream = bindingContext.ActionContext?.HttpContext?.Request?.Body;

            if (inputStream == null || !inputStream.CanRead)
            {
                throw new Exception("HTTP context is null or body can't be read");
            }

            bindingContext.Result = ModelBindingResult.Success(parser.Read(inputStream));

            return Task.CompletedTask;
        }

        private IRequestFormatter GetParserFromContext(HttpContext httpContext)
        {
            switch (httpContext.Request.ContentType.ToLower())
            {
                case "application/xml":
                case "text/xml":
                    return new XmlRequestFormatter();
                case "application/json":// TODO: handle JSON case
                    throw new NotImplementedException();
                default:
                    throw new Exception($"Content-Type '{httpContext.Request.ContentType}' is not supported.");
            }
        }
    }
}
