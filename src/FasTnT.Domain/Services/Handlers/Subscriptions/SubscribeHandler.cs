using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SubscribeHandler : ISubscriptionHandler<Subscription>
    {
        public async Task<IEpcisResponse> Handle(Subscription subscribe)
        {
            // TODO: handle and store subscription request.
            return await Task.FromResult(new SubscribeResponse());
        }
    }
}
