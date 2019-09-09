using FasTnT.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IEventManager
    {
        Task Store(int requestId, IEnumerable<EpcisEvent> events, CancellationToken cancellationToken);
    }
}
