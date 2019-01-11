using FasTnT.Domain.Services.Dispatch;
using FasTnT.Model.Queries;
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
            var query = new Poll { /*Parameters = subscription.Parameters*/ };
            var result = await _dispatcher.Dispatch(query);

            await _resultSender.Send(subscription.Destination, result);
        }
    }

    public interface ISubscriptionResultSender
    {
        Task Send(string destination, IEpcisResponse response);
    }
}
