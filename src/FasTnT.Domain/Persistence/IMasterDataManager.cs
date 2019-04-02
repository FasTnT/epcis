using FasTnT.Model.MasterDatas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IMasterDataManager
    {
        Task Store(Guid requestId, IEnumerable<EpcisMasterData> masterData);

        void WhereTypeIn(string[] values);
        void WhereIdIn(string[] values);
        void WhereIsDescendantOf(string[] values);
        void WhereAnyAttributeNamed(string[] values);
        void Limit(int limit);
        Task<IEnumerable<EpcisMasterData>> ToList(string[] attributes, bool includeChildren);
    }
}