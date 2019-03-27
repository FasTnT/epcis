using System;

namespace FasTnT.Model
{
    public class EpcisRequestHeader
    {
        public DateTime DocumentTime { get; set; }
        public DateTime RecordTime { get; set; } = DateTime.UtcNow;
        public string SchemaVersion { get; set; }
    }
}
