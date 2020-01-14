using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IEventFetcher
    {
        void Apply(SimpleParameterFilter filter);
        void Apply(ComparisonParameterFilter filter);
        void Apply(BusinessTransactionFilter filter);
        void Apply(MatchEpcFilter filter);
        void Apply(LimitFilter filter);
        void Apply(QuantityFilter filter);
        void Apply(CustomFieldFilter filter);
        void Apply(ExistCustomFieldFilter filter);
        Task<IEnumerable<EpcisEvent>> Fetch(CancellationToken cancellationToken);
    }
}
