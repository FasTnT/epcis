using FasTnT.Domain;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public interface IEpcisQuery
    {
        string Name { get; }
        bool AllowSubscription { get; }

        void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false);
        Task<IEnumerable<EpcisEvent>> Execute(IEnumerable<QueryParameter> parameters, IEventRepository query);
    }
}
