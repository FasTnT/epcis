using FasTnT.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEpcisDomain(this IServiceCollection services)
        {
            services.AddScoped<RequestContext>();
            services.AddScoped<IEpcisQuery, SimpleEventQuery>();
            services.AddScoped<IEpcisQuery, SimpleMasterdataQuery>();

            return services;
        }
    }
}
