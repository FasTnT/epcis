using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisMasterdataDocument : Request
    {
        public IList<EpcisMasterData> MasterDataList { get; set; } = new List<EpcisMasterData>();
        public IList<EpcisMasterDataHierarchy> HierarchyList { get; set; } = new List<EpcisMasterDataHierarchy>();
    }
}
