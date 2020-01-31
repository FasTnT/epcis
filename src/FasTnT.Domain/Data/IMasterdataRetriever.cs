using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IMasterdataFetcher
    {
        void Apply(LimitFilter filter);
        void Apply(MasterdataTypeFilter masterdataTypeFilter);
        void Apply(MasterdataNameFilter masterdataNameFilter);
        void Apply(MasterdataExistsAttibuteFilter masterdataExistsAttibuteFilter);
        void Apply(MasterdataDescendentNameFilter masterdataDescendentNameFilter);
        Task<IEnumerable<EpcisMasterData>> Fetch(string[] attributes, bool includeChildren, CancellationToken cancellationToken);
    }
}
