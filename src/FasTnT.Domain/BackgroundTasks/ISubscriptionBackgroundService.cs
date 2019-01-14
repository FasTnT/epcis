using FasTnT.Model.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.BackgroundTasks
{
    public interface ISubscriptionBackgroundService
    {
        void Register(Subscription subscription);
        void Remove(Subscription subscription);
        Task Run(CancellationToken cancellationToken);
    }
}
