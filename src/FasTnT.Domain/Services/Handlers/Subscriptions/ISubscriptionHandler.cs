using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public interface ISubscriptionHandler<T>
    {
        Task<IEpcisResponse> Handle(T query);
    }
}
