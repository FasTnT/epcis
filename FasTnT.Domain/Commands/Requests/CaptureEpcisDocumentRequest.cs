using FasTnT.Commands.Responses;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using MediatR;
using System.Collections.Generic;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentRequest : IRequest<IEpcisResponse>
    {
        public EpcisRequestHeader Header { get; set; }
        public IList<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public IList<EpcisMasterData> MasterDataList { get; set; } = new List<EpcisMasterData>();
    }
}
