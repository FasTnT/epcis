using FasTnT.Domain.BackgroundTasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.BackgroundTasks
{
    public class SubscriptionService : BackgroundService
    {
        private readonly ISubscriptionBackgroundService _subscriptionBackgroundService;

        public SubscriptionService(ISubscriptionBackgroundService subscriptionBackgroundService)
        {
            _subscriptionBackgroundService = subscriptionBackgroundService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _subscriptionBackgroundService.Run(cancellationToken);
        }
    }
}
