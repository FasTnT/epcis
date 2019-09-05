using FasTnT.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IEventStore
    {
        Task Store(int requestId, IEnumerable<EpcisEvent> events, CancellationToken cancellationToken);
    }
}
