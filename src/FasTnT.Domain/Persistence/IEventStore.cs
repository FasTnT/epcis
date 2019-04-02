using FasTnT.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IEventStore
    {
        Task Store(Guid requestId, IEnumerable<EpcisEvent> events);
    }
}
