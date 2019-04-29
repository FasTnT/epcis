using FasTnT.Model;
using FasTnT.Model.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IRequestStore
    {
        Task<Guid> Store(EpcisRequestHeader request, User user, CancellationToken cancellationToken);
    }
}
