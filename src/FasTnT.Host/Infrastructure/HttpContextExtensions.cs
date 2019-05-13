using FasTnT.Domain;
using FasTnT.Formatters;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static async Task SetEpcisResponse(this HttpContext context, IEpcisResponse response, int statusCode, CancellationToken cancellationToken)
        {
            var formatter = GetResponseFormatter(context);

            context.Response.ContentType = formatter.ToContentTypeString();
            context.Response.StatusCode = statusCode;
            await formatter.Write(response, context.Response.Body, cancellationToken);
        }

        private static IResponseFormatter GetResponseFormatter(HttpContext context)
        {
            var contentType = GetContentType(context);
            var formatterFactory = context.RequestServices.GetService<FormatterProvider>();

            return formatterFactory.GetFormatter<IEpcisResponse>(contentType) as IResponseFormatter;
        }

        private static string GetContentType(HttpContext context)
            => (context.Request.Headers.TryGetValue("Accept", out StringValues accept) ? accept.FirstOrDefault(x => x != "*/*") : null) ?? context.Request.ContentType;
    }
}
