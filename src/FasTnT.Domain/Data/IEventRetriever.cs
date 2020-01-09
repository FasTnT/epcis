using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IEventFetcher
    {
        void Apply(SimpleParameterFilter filter);
        Task<EpcisEvent[]> Fetch(CancellationToken cancellationToken);
    }
}
