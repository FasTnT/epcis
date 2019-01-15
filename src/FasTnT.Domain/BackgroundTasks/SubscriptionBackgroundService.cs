using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Model.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private ConcurrentDictionary<string, IList<Subscription>> _triggeredSubscriptions = new ConcurrentDictionary<string, IList<Subscription>>();
        private ConcurrentQueue<string> _triggeredValues = new ConcurrentQueue<string>();

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
                var triggeredSubscriptions = new List<Subscription>();
                try
                {
                    // Get all subscriptions where the next execution time is reached
                    var subscriptions = _scheduledExecutions.Where(x => x.Value <= DateTime.UtcNow);
                    subscriptions.ForEach(x => _scheduledExecutions.TryUpdate(x.Key, new SubscriptionSchedule(x.Key).GetNextOccurence(DateTime.UtcNow), x.Value));

                    triggeredSubscriptions.AddRange(subscriptions.Select(x => x.Key));

                    // Get all subscriptions scheduled by a trigger
                    while (_triggeredValues.TryDequeue(out string trigger)) triggeredSubscriptions.AddRange(_triggeredSubscriptions[trigger]);

                    await Execute(triggeredSubscriptions);
                }
                finally
                {
                    WaitTillNextExecutionOrNotification();
                }
            }
        }

        private async Task Execute(IEnumerable<Subscription> subscriptions)
        {
            using (var scope = _services.CreateScope())
            {
                var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();
                await Task.WhenAll(subscriptions.Select(s => subscriptionRunner.Run(s)));
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
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(subscription.Trigger))
                {
                    _scheduledExecutions[subscription] = new SubscriptionSchedule(subscription).GetNextOccurence(DateTime.UtcNow);
                }
                else
                {
                    if (!_triggeredSubscriptions.ContainsKey(subscription.Trigger))
                    {
                        _triggeredSubscriptions[subscription.Trigger] = new List<Subscription>();
                    }

                    _triggeredSubscriptions[subscription.Trigger].Add(subscription);
                }
            });
        }

        public void Remove(Subscription subscription)
        {
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(subscription.Trigger))
                {
                    _scheduledExecutions.TryRemove(_scheduledExecutions.SingleOrDefault(x => x.Key.SubscriptionId == subscription.SubscriptionId).Key, out DateTime value);
                }
                else
                {
                    _triggeredSubscriptions[subscription.Trigger].Remove(_triggeredSubscriptions[subscription.Trigger].SingleOrDefault(x => x.SubscriptionId == subscription.SubscriptionId));
                }
            });
        }

        public void Trigger(string triggerName)
        {
            Pulse(() => _triggeredValues.Enqueue(triggerName));
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

                if (timeUntilTrigger > TimeSpan.Zero) Monitor.Wait(_monitor, timeUntilTrigger);
            }
        }
    }
}
