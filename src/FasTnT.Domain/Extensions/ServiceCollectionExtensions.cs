using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Services;
using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Domain.Services.Users;
using FasTnT.Model.Queries.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace FasTnT.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEpcisDomain(this IServiceCollection services)
        {
            var queries = Assembly.GetAssembly(typeof(IEpcisQuery)).ExportedTypes.Where(x => x.GetInterfaces().Any(i => i == typeof(IEpcisQuery))).Select(x => Activator.CreateInstance(x)).Cast<IEpcisQuery>().ToArray();
            services.AddSingleton(typeof(IEpcisQuery[]), queries);

            services.AddScoped<CaptureService>();
            services.AddScoped<QueryService>();
            services.AddScoped<SubscriptionService>();
            services.AddScoped<CallbackService>();
            services.AddScoped<UserContext>();
            services.AddScoped<ISubscriptionResultSender, HttpSubscriptionResultSender>();
            services.AddScoped<SubscriptionRunner>();
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionBackgroundService>();
        }
    }
}
