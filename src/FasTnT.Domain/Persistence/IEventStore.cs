using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IEventStore
    {
        Task Store(EpcisEventDocument eventDocument);
    }
}
