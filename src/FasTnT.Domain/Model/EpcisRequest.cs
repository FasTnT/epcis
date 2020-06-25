using FasTnT.Model.Events;
using FasTnT.Model.Headers;
using FasTnT.Model.MasterDatas;
using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisRequest
    {
        public StandardBusinessHeader StandardBusinessHeader { get; set; }
        public DateTime DocumentTime { get; set; }
        public DateTime RecordTime { get; set; } = DateTime.UtcNow;
        public string SchemaVersion { get; set; }
        public SubscriptionCallback SubscriptionCallback { get; set; }
        public List<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public List<EpcisMasterData> MasterdataList { get; set; } = new List<EpcisMasterData>();
    }
}
