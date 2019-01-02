using System.Collections.Generic;
using System.Threading.Tasks;
using FasTnT.Model.Queries.Implementations.PredefinedQueries;

namespace FasTnT.Domain.Services.Handlers.PredefinedQueries
{
    public interface ISimpleEventQueryExecutor
    {
        Task<IEnumerable<EpcisEvent>> Execute(SimpleEventQuery query);
    }
}
