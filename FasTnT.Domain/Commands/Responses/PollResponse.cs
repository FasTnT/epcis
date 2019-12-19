using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Commands.Responses
{
    public class PollResponse : IEpcisResponse
    {
        public string QueryName { get; set; }
        public string SubscriptionId { get; set; }
        public IList<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public IList<EpcisMasterData> MasterdataList { get; set; } = new List<EpcisMasterData>();
    }
}
