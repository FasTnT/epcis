using System;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FasTnT.Host
{
    public class EpcisResponseOutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return context.ObjectType == typeof(IEpcisResponse);
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            var formatter = GetEpcisFormatter(context);
            var rawResponse = context.Object as IEpcisResponse;

            context.HttpContext.Response.ContentType = formatter.ToContentTypeString();
            formatter.Write(rawResponse, context.HttpContext.Response.Body);

            return Task.CompletedTask;
        }

        private static IResponseFormatter GetEpcisFormatter(OutputFormatterWriteContext context)
        {
            var requestAcceptedContentType = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();
            if (string.IsNullOrEmpty(requestAcceptedContentType) || requestAcceptedContentType == "*/*")
            {
                // Default to XML.
                requestAcceptedContentType = context.ContentType.HasValue ? context.ContentType.Value : "text/xml";
            }

            switch (requestAcceptedContentType.ToLower())
            {
                case "application/json":
                    throw new NotImplementedException();
                case "application/xml":
                case "text/xml":
                    return new XmlResponseFormatter();
                default:
                    throw new Exception($"Content-Type '{requestAcceptedContentType}' is not supported.");
            }
        }
    }
}
