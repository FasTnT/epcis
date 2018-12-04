using FasTnT.Host.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Persistence.Dapper;
using FasTnT.Domain;
using FasTnT.Host.Middleware;
using FasTnT.Host.Infrastructure.Attributes;

namespace FasTnT.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEpcisDomain();
            services.AddEpcisPersistence(Configuration.GetConnectionString("FasTnT.Database"));
            services.AddMvc(opt =>
            {
                opt.OutputFormatters.Insert(0, new EpcisResponseOutputFormatter());
                opt.ModelBinderProviders.Insert(0, new EpcisInputBinderProvider());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandlingMiddleware()
               .UseMvc();

            DevelopmentOnlyAttribute.Configuration = () => env;
        }
    }
}
