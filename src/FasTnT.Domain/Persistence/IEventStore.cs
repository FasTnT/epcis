using FasTnT.Model;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IEventStore
    {
        Task Store(Guid requestId, EpcisEvent[] events);
    }

    public interface IRequestStore
    {
        Task Store(EpcisRequestHeader request);
    }
}
