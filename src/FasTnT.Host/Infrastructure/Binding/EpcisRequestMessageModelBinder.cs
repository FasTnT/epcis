using FasTnT.Formatters;
using FasTnT.Formatters.Json;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisRequestModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.HttpContext;
            var parser = GetParserFromContentType(httpContext);
            var model = await parser.Read(bindingContext.HttpContext.Request.Body, bindingContext.HttpContext.RequestAborted);

            bindingContext.Result = ModelBindingResult.Success(model);
        }

        private static IRequestFormatter GetParserFromContentType(HttpContext context)
        {
            switch (context.Request.ContentType.Split(';').First().Trim())
            {
                case "application/xml":
                case "text/xml":
                    context.SetFormatter(new XmlResponseFormatter());
                    return new XmlRequestFormatter();
                case "application/json":
                    context.SetFormatter(new JsonResponseFormatter());
                    return new JsonRequestFormatter();
                case "application/pkcs7-mime": // TODO: AS2
                default:
                    throw new Exception($"Invalid content-type for Query parser: '{context.Request.ContentType}'");
            }
        }
    }
}
