using System.Collections.Generic;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Domain.Data.Model
{
    public class CaptureDocumentRequest
    {
        public EpcisRequestHeader Header { get; set; }
        public IList<EpcisEvent> EventList { get; set; }
        public IList<EpcisMasterData> MasterdataList { get; set; }
    }
}
