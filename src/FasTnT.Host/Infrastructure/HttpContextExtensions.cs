using System;
using System.Linq;
using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static void SetEpcisResponse(this HttpContext context, IEpcisResponse response)
        {
            var formatter = GetEpcisFormatter(context);

            context.Response.ContentType = formatter.ToContentTypeString();
            formatter.Write(response, context.Response.Body);
        }

        private static IResponseFormatter GetEpcisFormatter(HttpContext context)
        {
            var requestAcceptedContentType = context.Request.Headers["Accept"].FirstOrDefault();
            if (string.IsNullOrEmpty(requestAcceptedContentType) || requestAcceptedContentType == "*/*")
            {
                // Default to XML.
                requestAcceptedContentType = !string.IsNullOrWhiteSpace(context.Request.ContentType) ? context.Request.ContentType : "text/xml";
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
