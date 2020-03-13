using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Host.Middleware;
using Microsoft.Extensions.Hosting;
using FasTnT.Host.Infrastructure.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using MediatR;
using FasTnT.Domain;
using FasTnT.Domain.Queries;
using FasTnT.Data.PostgreSql;
using FasTnT.Subscriptions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FasTnT.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
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
            services.AddMediatR(Constants.Assembly);
            services.AddScoped<RequestContext>();
            // Add Domain services
            services.AddScoped<IEpcisQuery, SimpleEventQuery>();
            services.AddScoped<IEpcisQuery, SimpleMasterdataQuery>();

            // Add Storage services
            services.AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"))
                    .AddBackgroundSubscriptionService();

            services.AddMvc(ConfigureMvsOptions)
                    .AddApplicationPart(typeof(Startup).Assembly)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandlingMiddleware(env.IsDevelopment())
               .UseAuthentication()
               .UseOkStatusCode()
               .UseMvc();
        }

        private static void ConfigureMvsOptions(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EpcisModelBinderProvider());
            options.OutputFormatters.Insert(0, new EpcisResponseOutputFormatter());
        }
    }
}
