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
using FasTnT.Domain.BackgroundTasks;
using FasTnT.Host.Middleware.Authentication;
using FasTnT.Domain;
using FasTnT.Formatters.Json;

namespace FasTnT.Host
{
    public class Startup
    {
        public static string EpcisServicePath = "/EpcisServices/1.2";
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEpcisDomain();
            services.AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"));
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundService>();
            services.AddSingleton(new FormatterProvider(new IFormatterFactory[]{ new JsonFormatterFactory(), new XmlFormatterFactory(), new SoapFormatterFactory() }));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            SubscriptionBackgroundService.DelayTimeoutInMs = Configuration.GetSection("Settings").GetValue("SubscriptionWaitTimeout", 5000);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                   .UseEpcisMigrationEndpoint($"{EpcisServicePath}/Database");
            }

            app.UseExceptionHandlingMiddleware()
               .UseWhen(context => context.Request.Path.StartsWithSegments(EpcisServicePath), x => {
                    x.UseBasicAuthentication("FasTnT")
                     .UseEpcisCaptureEndpoint($"{EpcisServicePath}/Capture")
                     .UseEpcisQueryEndpoint($"{EpcisServicePath}/Query")
                     .UseEpcisSubscriptionTrigger($"{EpcisServicePath}/Subscription/Trigger");
                });
        }
    }
}
