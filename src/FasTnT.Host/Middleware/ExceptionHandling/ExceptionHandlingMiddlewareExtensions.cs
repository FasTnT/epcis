using Microsoft.AspNetCore.Builder;

namespace FasTnT.Host.Middleware
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
