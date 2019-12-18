using FasTnT.Commands.Responses;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisResponseOutputFormatter : OutputFormatter
    {
        public EpcisResponseOutputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
            SupportedMediaTypes.Add("application/xml");
            SupportedMediaTypes.Add("text/xml");
        }

        protected override bool CanWriteType(Type type) => typeof(IEpcisResponse).IsAssignableFrom(type);

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var httpCtx = context.HttpContext;
            httpCtx.Response.ContentType = httpCtx.GetFormatter().ContentType;
            await httpCtx.GetFormatter().WriteResponse(context.Object as IEpcisResponse, httpCtx.Response.Body, httpCtx.RequestAborted);
        }
    }
}
