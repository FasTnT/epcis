using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public class GetSubscriptionIdsHandler : IQueryHandler<GetSubscriptionIds>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSubscriptionIdsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEpcisResponse> Handle(GetSubscriptionIds query)
        {
            var subscriptions = (await _unitOfWork.SubscriptionManager.GetAll()).Where(s => s.QueryName == query.QueryName);

            return new GetSubscriptionIdsResult
            {
                SubscriptionIds = subscriptions.Select(x => x.SubscriptionId).ToArray()
            };
        }
    }
}
