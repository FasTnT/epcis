using FasTnT.Domain.Commands;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host
{
    public static class HttpContextExtensions
    {
        private const string FormatterKey = "Formatter";

        public static ICommandParser GetFormatter(this HttpContext context) => context.Items[FormatterKey] as ICommandParser;
        public static void SetFormatter(this HttpContext context, ICommandParser formatter) => context.Items[FormatterKey] = formatter;
    }
}
