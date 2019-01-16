using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Subscriptions
{
    public interface ISubscriptionHandler<T>
    {
        Task<IEpcisResponse> Handle(T query);
    }
}
