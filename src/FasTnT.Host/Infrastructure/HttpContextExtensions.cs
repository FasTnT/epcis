using FasTnT.Domain;
using FasTnT.Formatters;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static async Task SetEpcisResponse(this HttpContext context, IEpcisResponse response, CancellationToken cancellationToken)
        {
            var formatter = GetResponseFormatter(context);
            context.Response.ContentType = formatter.ToContentTypeString();
            await formatter.Write(response, context.Response.Body, cancellationToken);
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
