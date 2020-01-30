using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IEventFetcher
    {
        void Apply(RequestIdFilter requestIdFilter);
        void Apply<T>(SimpleParameterFilter<T> filter);
        void Apply(ComparisonParameterFilter filter);
        void Apply(BusinessTransactionFilter filter);
        void Apply(MatchEpcFilter filter);
        void Apply(LimitFilter filter);
        void Apply(QuantityFilter filter);
        void Apply(CustomFieldFilter filter);
        void Apply(ExistCustomFieldFilter filter);
        void Apply(ExistsErrorDeclarationFilter filter);
        void Apply(EqualsErrorReasonFilter filter);
        void Apply(EqualsCorrectiveEventIdFilter filter);
        void Apply(MasterdataHierarchyFilter filter);
        void Apply(SourceDestinationFilter filter);
        Task<IEnumerable<EpcisEvent>> Fetch(CancellationToken cancellationToken);
    }
}
