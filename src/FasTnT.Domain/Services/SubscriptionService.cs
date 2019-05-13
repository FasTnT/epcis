using FasTnT.Domain.BackgroundTasks;
using FasTnT.Model.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class SubscriptionService
    {
        private readonly ISubscriptionBackgroundService _backgroundService;

        public SubscriptionService(ISubscriptionBackgroundService backgroundService) => _backgroundService = backgroundService;

        public Task Process(TriggerSubscriptionRequest query, CancellationToken cancellationToken) => Task.Run(() => _backgroundService.Trigger(query.Trigger), cancellationToken);
    }
}
