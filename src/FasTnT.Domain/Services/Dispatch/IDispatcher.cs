using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Dispatch
{
    public interface IDispatcher
    {
        Task<IEpcisResponse> Dispatch(Request document);
        Task<IEpcisResponse> Dispatch(EpcisQuery query);
        Task<IEpcisResponse> Dispatch(SubscriptionRequest request);
    }
}
