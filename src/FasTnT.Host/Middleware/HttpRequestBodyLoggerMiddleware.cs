using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Host.Middleware
{
    public class HttpRequestBodyLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpRequestBodyLoggerMiddleware(RequestDelegate next) => _next = next;

        public Task Invoke(HttpContext context)
        {
            if(context.Request.ContentLength > 0)
            {
                LogRequestBody(context);
            }

            return _next(context);
        }

        private static void LogRequestBody(HttpContext context)
        {
            var logger = context.RequestServices.GetService<ILogger<HttpRequestBodyLoggerMiddleware>>();
            context.Request.EnableBuffering();

            using (var streamReader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                logger.LogInformation(streamReader.ReadToEnd());
            }

            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }
    }
}
