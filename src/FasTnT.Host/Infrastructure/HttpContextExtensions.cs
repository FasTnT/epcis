using FasTnT.Formatters;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static void SetEpcisResponse(this HttpContext context, IEpcisResponse response, IResponseFormatter formatter)
        {
            context.Response.ContentType = formatter.ToContentTypeString();
            formatter.Write(response, context.Response.Body);
        }
    }
}
