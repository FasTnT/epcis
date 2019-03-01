using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Persistence.Dapper;
using FasTnT.Host.Middleware;
using FasTnT.Host.BackgroundTasks;
using FasTnT.Domain.Extensions;
using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using Microsoft.Extensions.Logging;
using FasTnT.Domain.BackgroundTasks;
using FasTnT.Host.Middleware.Authentication;

namespace FasTnT.Host
{
    public class Startup
    {
        public static string Prefix = "/EpcisServices/1.2";
        public IConfiguration Configuration { get; }
        public ILoggerFactory LogFactory { get; }

        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            Configuration = configuration;
            LogFactory = logFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEpcisDomain();
            services.AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"));
            services.AddScoped(typeof(IResponseFormatter), typeof(XmlResponseFormatter)); // Use XML as default formatter for subscriptions.
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SubscriptionBackgroundService.DelayTimeoutInMs = Configuration.GetSection("Settings").GetValue("SubscriptionWaitTimeout", 5000);

            app.UseExceptionHandlingMiddleware()
                .UseBasicAuthentication("FasTnT")
                .UseEpcisCaptureEndpoint($"{Prefix}/Capture")
                .UseEpcisQueryEndpoint($"{Prefix}/Query")
                .UseEpcisSubscriptionTrigger($"{Prefix}/Subscription/Trigger")
                .UseEpcisMigrationEndpoint($"{Prefix}/Database", env.IsDevelopment());
        }
    }
}
