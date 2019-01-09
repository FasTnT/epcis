using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Dispatch
{
    public interface IDispatcher
    {
        Task<IEpcisResponse> Dispatch(Request document);
        Task<IEpcisResponse> Dispatch(EpcisQuery query);
    }
}
