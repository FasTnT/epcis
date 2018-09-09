using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SubscribeHandler : IQueryHandler<Subscribe>
    {
        public async Task<IEpcisResponse> Handle(Subscribe subscribe)
        {
            if (!subscribe.Query.AllowsSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Subscribe is not permitted");
            }

            return await Task.FromResult(new SubscribeResponse());
        }
    }
}
