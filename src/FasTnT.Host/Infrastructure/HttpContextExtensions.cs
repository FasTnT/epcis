using FasTnT.Formatters;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        private const string FormatterKey = "Formatter";

        public static IFormatter GetFormatter(this HttpContext context) => context.Items[FormatterKey] as IFormatter;
        public static void SetFormatter(this HttpContext context, IFormatter formatter) => context.Items[FormatterKey] = formatter;
    }
}
