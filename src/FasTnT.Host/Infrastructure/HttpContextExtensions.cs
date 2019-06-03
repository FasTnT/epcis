using FasTnT.Formatters;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        public static IFormatter GetFormatter(this HttpContext context) => context.Items["Formatter"] as IFormatter;
        public static void SetFormatter(this HttpContext context, IFormatter formatter) => context.Items["Formatter"] = formatter;
    }
}
