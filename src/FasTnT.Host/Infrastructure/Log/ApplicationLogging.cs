using Microsoft.Extensions.Logging;

namespace FasTnT.Host.Infrastructure.Log
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }
}
