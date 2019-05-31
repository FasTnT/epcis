using FasTnT.Formatters;
using FasTnT.Formatters.Json;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static async Task SetEpcisResponse(this HttpContext context, IEpcisResponse response, int statusCode, CancellationToken cancellationToken)
        {
            context.Response.ContentType = context.Request.ContentType;
            context.Response.StatusCode = statusCode;

            await context.GetFormatter().Write(response, context.Response.Body, cancellationToken);
        }

        public static async Task<Request> ParseXmlRequest(this HttpContext context) => await Parse(context, new XmlRequestFormatter());
        public static async Task<EpcisQuery> ParseSoapQuery(this HttpContext context) => await Parse(context, new SoapQueryFormatter());
        public static bool IsPost(this HttpContext context) => HttpMethods.IsPost(context.Request.Method);
        public static bool IsGet(this HttpContext context) => HttpMethods.IsGet(context.Request.Method);
        public static void SetFormatter(this HttpContext context, IResponseFormatter formatter) => context.Items["Formatter"] = formatter;

        public static IResponseFormatter GetFormatter(this HttpContext context)
        {
            return (context.Items["Formatter"] ?? new JsonResponseFormatter()) as IResponseFormatter;
        }

        public static async Task<T> Parse<T>(HttpContext context, IFormatter<T> formatter) where T : IEpcisPayload
        {
            return await formatter.Read(context.Request.Body, context.RequestAborted);
        }

        public static bool PathEquals(this HttpContext context, string path)
        {
            return context.Request.Path.Value.TrimEnd('/').Equals(path.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsXmlContentType(this HttpContext context)
        {
            return context.Request.ContentType.Contains("XML", StringComparison.OrdinalIgnoreCase);
        }
    }
}
