using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public interface IHandler<T>
    {
        Task<IEpcisResponse> Handle(T arg);
    }
}
