using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IMasterDataManager
    {
        Task Store(EpcisMasterData masterData);

        void WhereTypeIn(string[] values);
        void WhereIdIn(string[] values);
        void WhereAnyAttributeNamed(string[] values);
        void Limit(int limit);
        Task<IEnumerable<EpcisMasterData>> ToList(bool includeAttributes);
    }
}