using Microsoft.AspNetCore.Builder;

namespace FasTnT.Host
{
    public static class EpcisMiddlewareExtensions
    {
        public static IApplicationBuilder UseEpcisCaptureEndpoint(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisCaptureMiddleware>(path);
        public static IApplicationBuilder UseEpcisQueryEndpoint(this IApplicationBuilder app, string path) => app.UseMiddleware<EpcisQueryMiddleware>(path);
        public static IApplicationBuilder UseEpcisMigrationEndpoint(this IApplicationBuilder app, string path, bool isEnabled = true) => isEnabled ? app.UseMiddleware<EpcisMigrationMiddleware>(path) : app;
    }
}
