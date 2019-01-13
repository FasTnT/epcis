using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.BackgroundTasks
{
    public class SubscriptionService : IHostedService, IDisposable
    {
        public static int DelayTimeoutInMs { get; set; }

        private Task _executingTask;
        private readonly CancellationTokenSource _stopping = new CancellationTokenSource();
        private readonly IServiceProvider _services;

        public SubscriptionService(IServiceProvider services)
        {
            _services = services;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var subscriptionManager = scope.ServiceProvider.GetService<ISubscriptionManager>();
                    var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();
                    var subscriptions = await subscriptionManager.GetAll();

                    foreach(var subscription in subscriptions)
                    {
                        await subscriptionRunner.Run(subscription);
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(DelayTimeoutInMs));
            }

            Console.WriteLine("Successfully stopped subscription service");
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stopping.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stopping.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            _stopping.Cancel();
        }
    }
}
