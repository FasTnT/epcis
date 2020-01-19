using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Subscriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackgroundSubscriptionService(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, SubscriptionBackgroundService>();

            return services;
        }
    }
}
