using FasTnT.Host.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Persistence.Dapper;
using FasTnT.Host.Middleware;
using FasTnT.Host.Infrastructure.Attributes;
using FasTnT.Host.Infrastructure.Authentication;
using FasTnT.Host.BackgroundTasks;
using FasTnT.Domain.Extensions;
using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Domain.BackgroundTasks;
using Microsoft.AspNetCore.Authentication;

namespace FasTnT.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);
            services.AddEpcisDomain();
            services.AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"));
            services.AddScoped(typeof(IResponseFormatter), typeof(XmlResponseFormatter)); // Use XML as default formatter for subscriptions.
            services.AddMvc(opt =>
            {
                opt.OutputFormatters.Insert(0, new EpcisResponseOutputFormatter());
                opt.ModelBinderProviders.Insert(0, new EpcisInputBinderProvider());
            });

            services.AddSingleton<DevelopmentOnlyFilter>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // TODO: get from configuration?
            SubscriptionBackgroundService.DelayTimeoutInMs = 5000;

            app.UseExceptionHandlingMiddleware()
               .UseAuthentication()
               .UseMvc();
        }
    }
}
