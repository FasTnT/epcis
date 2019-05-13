using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class CaptureRequest : Request
    {
        public IList<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public IList<EpcisMasterData> MasterDataList { get; set; } = new List<EpcisMasterData>();
    }
}
