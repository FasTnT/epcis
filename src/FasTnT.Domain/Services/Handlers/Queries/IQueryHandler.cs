using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers.Queries
{
    public interface IQueryHandler<T>
    {
        Task<IEpcisResponse> Handle(T query);
    }
}
