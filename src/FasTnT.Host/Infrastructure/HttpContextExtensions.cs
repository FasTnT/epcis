using FasTnT.Domain;
using FasTnT.Formatters;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static void SetEpcisResponse(this HttpContext context, IEpcisResponse response)
        {
            var formatter = GetResponseFormatter(context);
            context.Response.ContentType = formatter.ToContentTypeString();
            formatter.Write(response, context.Response.Body);
        }

        private static IResponseFormatter GetResponseFormatter(HttpContext context)
        {
            var contentType = GetContentType(context);
            var formatterFactory = context.RequestServices.GetService<FormatterProvider>();

            return formatterFactory.GetFormatter<IEpcisResponse>(contentType) as IResponseFormatter;
        }

        private static string GetContentType(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Accept"))
            {
                var acceptValue = context.Request.Headers["Accept"].First();
                if (acceptValue != "*/*") return acceptValue;
            }

            return context.Request.ContentType;
        }
    }
}
