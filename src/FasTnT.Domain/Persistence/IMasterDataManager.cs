using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IMasterDataManager
    {
        Task Store(int requestId, IEnumerable<EpcisMasterData> masterData, CancellationToken cancellationToken);

        void WhereTypeIn(string[] values);
        void WhereIdIn(string[] values);
        void WhereIsDescendantOf(string[] values);
        void WhereAnyAttributeNamed(string[] values);
        void Limit(int limit);
        Task<IEnumerable<EpcisMasterData>> ToList(string[] attributes, bool includeChildren, CancellationToken cancellationToken);
    }
}