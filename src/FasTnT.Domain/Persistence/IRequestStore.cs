using FasTnT.Model;
using FasTnT.Model.Users;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IRequestStore
    {
        Task<int> Store(EpcisRequestHeader request, User user, CancellationToken cancellationToken);
    }
}
