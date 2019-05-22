using FasTnT.Domain;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using FasTnT.Model.Responses;

namespace FasTnT.Host.Infrastructure
{
    public class EpcisResponseOutputFormatter : OutputFormatter
    {
        public EpcisResponseOutputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
        }

        protected override bool CanWriteType(Type type) => typeof(IEpcisResponse).IsAssignableFrom(type);

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var httpCtx = context.HttpContext;
            var contentType = httpCtx.Response.ContentType;
            var formatterProvider = httpCtx.RequestServices.GetService<FormatterProvider>();
            var formatter = formatterProvider.GetFormatter<IEpcisResponse>(contentType);

            await formatter.Write(context.Object as IEpcisResponse, httpCtx.Response.Body, httpCtx.RequestAborted);
        }
    }
}