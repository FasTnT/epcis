using FasTnT.Domain.BackgroundTasks;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.BackgroundTasks
{
    public class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stopping = new CancellationTokenSource();
        private readonly ISubscriptionBackgroundService _subscriptionBackgroundService;

        public BackgroundService(ISubscriptionBackgroundService subscriptionBackgroundService)
        {
            _subscriptionBackgroundService = subscriptionBackgroundService;
        }

        protected async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _subscriptionBackgroundService.Run(cancellationToken);
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stopping.Token);
            
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null) return;

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
