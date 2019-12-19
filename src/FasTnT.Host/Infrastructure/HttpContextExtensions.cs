using FasTnT.Domain.Commands;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        private const string FormatterKey = "FasTnT.Epcis:Formatter";

        public static ICommandFormatter GetFormatter(this HttpContext context) => context.Items[FormatterKey] as ICommandFormatter;
        public static void SetFormatter(this HttpContext context, ICommandFormatter formatter) => context.Items[FormatterKey] = formatter;
    }
}
