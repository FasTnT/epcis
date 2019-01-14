using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Model.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.BackgroundTasks
{
    public class SubscriptionBackgroundService : ISubscriptionBackgroundService
    {
        public static int DelayTimeoutInMs { get; set; }

        private readonly IServiceProvider _services;
        private volatile object _monitor = new object();
        private ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new ConcurrentDictionary<Subscription, DateTime>();

        public SubscriptionBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await Initialize(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();

                        foreach (var entry in _scheduledExecutions.Where(x => x.Value <= DateTime.UtcNow))
                        {
                            _scheduledExecutions.TryUpdate(entry.Key, new SubscriptionSchedule(entry.Key).GetNextOccurence(DateTime.UtcNow), entry.Value);
                            await subscriptionRunner.Run(entry.Key);
                        }
                    }
                }
                finally
                {
                    WaitTillNextExecutionOrNotification();
                }
            }
        }

        //REVIEW: should this class be responsible to get all subscriptions at startup? (LAA)
        private async Task Initialize(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var subscriptionManager = scope.ServiceProvider.GetService<ISubscriptionManager>();
                var subscriptions = await subscriptionManager.GetAll(true);

                subscriptions.ForEach(Register);
            }
        }

        public void Register(Subscription subscription)
        {
            Pulse(() => _scheduledExecutions[subscription] = new SubscriptionSchedule(subscription).GetNextOccurence(DateTime.UtcNow));
        }

        public void Remove(Subscription subscription)
        {
            Pulse(() => _scheduledExecutions.TryRemove(_scheduledExecutions.SingleOrDefault(x => x.Key.SubscriptionId == subscription.SubscriptionId).Key, out DateTime value));
        }

        private void Pulse(Action action)
        {
            lock (_monitor)
            {
                action();
                Monitor.Pulse(_monitor);
            }
        }

        private void WaitTillNextExecutionOrNotification()
        {
            lock (_monitor)
            {
                var nextExecution = _scheduledExecutions.Any() ? _scheduledExecutions.Values.Min() : DateTime.UtcNow.AddMilliseconds(DelayTimeoutInMs);
                var timeUntilTrigger = nextExecution - DateTime.UtcNow;

                if (timeUntilTrigger > TimeSpan.Zero)
                {
                    Monitor.Wait(_monitor, timeUntilTrigger);
                }
            }
        }
    }
}
