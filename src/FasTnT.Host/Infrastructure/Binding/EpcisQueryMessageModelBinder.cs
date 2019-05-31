using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisQueryModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.HttpContext;
            var parser = GetParserFromContentType(httpContext);
            var model = await parser.Read(bindingContext.HttpContext.Request.Body, bindingContext.HttpContext.RequestAborted);

            bindingContext.Result = ModelBindingResult.Success(model);
        }

        private static IQueryFormatter GetParserFromContentType(HttpContext context)
        {
            switch (context.Request.ContentType.Split(';').First().Trim())
            {
                case "application/xml":
                case "text/xml":
                    context.SetFormatter(new SoapResponseFormatter());
                    return new SoapQueryFormatter();
                case "application/pkcs7-mime": // TODO: AS2
                default:
                    throw new Exception($"Invalid content-type for Query parser: '{context.Request.ContentType}'");
            }
        }
    }
}
