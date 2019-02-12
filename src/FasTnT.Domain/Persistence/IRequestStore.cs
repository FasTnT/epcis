using FasTnT.Model;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IRequestStore
    {
        Task Store(EpcisRequestHeader request);
    }
}
