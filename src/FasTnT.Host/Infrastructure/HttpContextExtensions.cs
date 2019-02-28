using System;
using System.Linq;
using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Host.Infrastructure;
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
            return HttpFormatterFactory.Instance.GetFormatter<IEpcisResponse>(context) as IResponseFormatter;
        }
    }
}
