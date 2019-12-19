using FasTnT.Domain.Commands;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentRequest : ICaptureRequest
    {
        public EpcisRequestHeader Header { get; set; }
        public IList<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public IList<EpcisMasterData> MasterDataList { get; set; } = new List<EpcisMasterData>();
    }
}
