using FasTnT.Domain.Services.Dispatch;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly ISubscriptionResultSender _resultSender;
        private readonly IDispatcher _dispatcher;

        public SubscriptionRunner(ISubscriptionResultSender resultSender, IDispatcher dispatcher)
        {
            _resultSender = resultSender;
            _dispatcher = dispatcher;
        }

        public async Task Run(Subscription subscription)
        {
            // TODO: perform query
            await _resultSender.Send(subscription.Destination, default(IEpcisResponse));
        }
    }

    public interface ISubscriptionResultSender
    {
        Task Send(string destination, IEpcisResponse response);
    }
}
