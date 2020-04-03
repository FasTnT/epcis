using FasTnT.Commands.Responses;
using FasTnT.Domain;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisResponseOutputFormatter : OutputFormatter
    {
        public EpcisResponseOutputFormatter()
        {
            SupportedMediaTypes.Add("application/xml");
            SupportedMediaTypes.Add("application/soap+xml");
            SupportedMediaTypes.Add("text/xml");
        }

        protected override bool CanWriteType(Type type) => typeof(IEpcisResponse).IsAssignableFrom(type);

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var httpContext = context.HttpContext;
            var requestContext = httpContext.RequestServices.GetService<RequestContext>();

            httpContext.Response.ContentType = requestContext.Formatter.ContentType;
            await requestContext.Formatter.WriteResponse(context.Object as IEpcisResponse, httpContext.Response.Body, httpContext.RequestAborted);
        }
    }
}
