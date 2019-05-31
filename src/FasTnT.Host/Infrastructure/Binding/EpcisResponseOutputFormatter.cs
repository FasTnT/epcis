using FasTnT.Formatters;
using FasTnT.Formatters.Json;
using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisResponseOutputFormatter : OutputFormatter
    {
        public EpcisResponseOutputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }
        protected override bool CanWriteType(Type type) => typeof(IEpcisResponse).IsAssignableFrom(type);

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var formatter = default(IResponseFormatter);

            if (context.ContentType.Value.Contains("xml"))
            {
                formatter = new SoapResponseFormatter();
            }
            else if (context.ContentType.Value.Contains("json"))
            {
                formatter = new JsonResponseFormatter();
            }

            await formatter.Write(context.Object as IEpcisResponse, context.HttpContext.Response.Body, context.HttpContext.RequestAborted);
        }
    }
}
