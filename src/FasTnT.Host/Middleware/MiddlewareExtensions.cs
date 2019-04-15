using FasTnT.Host.Middleware.Authentication;
using Microsoft.AspNetCore.Builder;

namespace FasTnT.Host.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder, bool isDevelopment = false) => builder.UseMiddleware<ExceptionHandlingMiddleware>(isDevelopment);
        public static IApplicationBuilder UseEpcisCaptureEndpoint(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisCaptureMiddleware>(path);
        public static IApplicationBuilder UseEpcisQueryEndpoint(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisQueryMiddleware>(path);
        public static IApplicationBuilder UseEpcisSubscriptionTrigger(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisSubscriptionTriggerMiddleware>(path);
        public static IApplicationBuilder UseEpcisMigrationEndpoint(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisMigrationMiddleware>(path);
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, string realm) => app.UseMiddleware<BasicAuthenticationMiddleware>(realm);
    }
}
