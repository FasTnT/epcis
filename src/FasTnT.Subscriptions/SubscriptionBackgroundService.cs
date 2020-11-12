using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using FasTnT.Subscriptions.Schedule;

namespace FasTnT.Subscriptions
{
    public sealed class SubscriptionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly object _monitor = new object();
        private readonly ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new ConcurrentDictionary<Subscription, DateTime>();
        private readonly ConcurrentDictionary<string, IList<Subscription>> _triggeredSubscriptions = new ConcurrentDictionary<string, IList<Subscription>>();
        private readonly ConcurrentQueue<string> _triggeredValues = new ConcurrentQueue<string>();

        public SubscriptionBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () => {
                await Initialize(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var triggeredSubscriptions = new List<Subscription>();
                    try
                    {
                        // Get all subscriptions where the next execution time is reached
                        var subscriptions = _scheduledExecutions.Where(x => x.Value <= DateTime.UtcNow).ToArray();
                        subscriptions.ForEach(x => _scheduledExecutions.TryUpdate(x.Key, new SubscriptionSchedule(x.Key.Schedule).GetNextOccurence(DateTime.UtcNow), x.Value));

                        triggeredSubscriptions.AddRange(subscriptions.Select(x => x.Key));

                        // Get all subscriptions scheduled by a trigger
                        while (_triggeredValues.TryDequeue(out string trigger))
                        {
                            triggeredSubscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out IList<Subscription> sub) ? sub : Array.Empty<Subscription>());
                        }

                        await Execute(triggeredSubscriptions, cancellationToken);
                    }
                    finally
                    {
                        WaitTillNextExecutionOrNotification();
                    }
                }
            });
        }

        private async Task Execute(IEnumerable<Subscription> subscriptions, CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();

            foreach (var subscription in subscriptions)
            {
                await subscriptionRunner.Run(subscription, cancellationToken);
            }
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            var initialized = false;

            while (!initialized)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var subscriptionManager = scope.ServiceProvider.GetService<ISubscriptionManager>();
                    var subscriptions = await subscriptionManager.GetAll(cancellationToken);

                    subscriptions.ForEach(Register);
                    initialized = true;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10000), cancellationToken);
                }
            }
        }

        public void Register(Subscription subscription)
        {
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(subscription.Trigger))
                {
                    _scheduledExecutions[subscription] = new SubscriptionSchedule(subscription.Schedule).GetNextOccurence(DateTime.UtcNow);
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
                    if (_scheduledExecutions.Any(x => x.Key.SubscriptionId == subscription.SubscriptionId))
                    {
                        _scheduledExecutions.TryRemove(_scheduledExecutions.FirstOrDefault(x => x.Key.SubscriptionId == subscription.SubscriptionId).Key, out DateTime value);
                    }
                }
                else
                {
                    _triggeredSubscriptions[subscription.Trigger].Remove(_triggeredSubscriptions[subscription.Trigger].FirstOrDefault(x => x.SubscriptionId == subscription.SubscriptionId));
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
                _ = _scheduledExecutions.Any()
                 ? Monitor.Wait(_monitor, GetNextExecutionWaitTime(_scheduledExecutions.Values.Min()))
                 : Monitor.Wait(_monitor);
            }
        }

        private TimeSpan GetNextExecutionWaitTime(DateTime executionTime)
            => executionTime < DateTime.UtcNow ? TimeSpan.Zero : executionTime - DateTime.UtcNow;
    }
}
