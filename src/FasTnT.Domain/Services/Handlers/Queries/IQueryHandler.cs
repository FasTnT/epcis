using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public interface IQueryHandler<T>
    {
        Task<IEpcisResponse> Handle(T query);
    }
}
