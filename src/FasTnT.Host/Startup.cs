using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Host.Middleware;
using Microsoft.Extensions.Hosting;
using FasTnT.Host.Infrastructure.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using MediatR;
using Npgsql;
using System;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.PostgreSql;
using FasTnT.PostgreSql.Capture;
using FasTnT.PostgreSql.Migration;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using FasTnT.Domain.Queries;
using FasTnT.Data.PostgreSql.DataRetrieval;
using FasTnT.Data.PostgreSql.Subscriptions;

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

            // Add Storage services
            services.AddScoped(OpenConnection);
            services.AddScoped<ITransactionProvider, TransactionProvider>();
            services.AddScoped<IDocumentStore, DocumentStore>();
            services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();
            services.AddScoped<IEventFetcher, EventFetcher>();
            services.AddScoped<ISubscriptionManager, SubscriptionManager>();

            // Add Domain services
            services.AddScoped<IEpcisQuery, SimpleEventQuery>();
            services.AddScoped<IEpcisQuery, SimpleMasterdataQuery>();

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
               .UseNoContentStatusCode()
               .UseMvc();
        }

        private static void ConfigureMvsOptions(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EpcisModelBinderProvider());
            options.OutputFormatters.Insert(0, new EpcisResponseOutputFormatter());
        }

        private IDbConnection OpenConnection(IServiceProvider serviceProvider)
        {
            var conn = new NpgsqlConnection(Configuration.GetConnectionString("FasTnT.Database"));
            conn.Open();

            return conn;
        }
    }
}
