using FasTnT.Model.Events;
using FasTnT.Model.Headers;
using FasTnT.Model.MasterDatas;
using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisRequest
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public StandardBusinessHeader StandardBusinessHeader { get; set; }
        public DateTime DocumentTime { get; set; }
        public DateTime RecordTime { get; set; } = DateTime.UtcNow;
        public string SchemaVersion { get; set; }
        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
        public List<EpcisEvent> EventList { get; set; }
        public List<EpcisMasterData> MasterdataList { get; set; }
    }
}
