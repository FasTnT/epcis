using FasTnT.Formatters;
using FasTnT.Host.Infrastructure;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static void SetEpcisResponse(this HttpContext context, IEpcisResponse response)
        {
            var formatter = HttpFormatterFactory.Instance.GetFormatter<IEpcisResponse>(context) as IResponseFormatter;

            context.Response.ContentType = formatter.ToContentTypeString();
            formatter.Write(response, context.Response.Body);
        }
    }
}
