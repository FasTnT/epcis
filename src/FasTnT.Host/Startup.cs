using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Host.Middleware;
using FasTnT.Host.Infrastructure.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using FasTnT.Domain;
using FasTnT.Data.PostgreSql;
using FasTnT.Subscriptions;

namespace FasTnT.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddApplicationInsightsTelemetry();

            services.AddEpcisDomain()
                    .AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"))
                    .AddBackgroundSubscriptionService();

            services.AddControllers(ConfigureOptions)
                    .AddApplicationPart(typeof(Startup).Assembly)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandlingMiddleware(env.IsDevelopment())
               .UseHttpSynchronousIO()
               .UseRequestBodyLogger()
               .UseRouting()
               .UseOkStatusCode()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }

        private static void ConfigureOptions(MvcOptions options)
        {
            options.EnableEndpointRouting = false;
            options.ModelBinderProviders.Insert(0, new EpcisModelBinderProvider());
            options.OutputFormatters.Insert(0, new EpcisResponseOutputFormatter());
        }
    }
}
