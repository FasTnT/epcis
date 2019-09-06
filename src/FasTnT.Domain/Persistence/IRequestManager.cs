using FasTnT.Model;
using FasTnT.Model.Users;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IRequestManager
    {
        Task<int> Store(EpcisRequestHeader request, User user, CancellationToken cancellationToken);
    }
}
