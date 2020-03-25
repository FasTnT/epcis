using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Host.Middleware;
using FasTnT.Host.Infrastructure.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using MediatR;
using FasTnT.Domain;
using FasTnT.Data.PostgreSql;
using FasTnT.Subscriptions;

namespace FasTnT.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets(GetType().Assembly);
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Constants.Assembly)
                    .AddEpcisDomain()
                    .AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"))
                    .AddBackgroundSubscriptionService();

            services.AddControllers(ConfigureOptions)
                    .AddApplicationPart(typeof(Startup).Assembly)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandlingMiddleware(env.IsDevelopment())
               .UseRouting()
               .UseNoContentStatusCode()
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
