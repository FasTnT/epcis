using System;

namespace FasTnT.Model
{
    public class EpcisRequestHeader
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DocumentTime { get; set; }
        public DateTime RecordTime { get; set; } = DateTime.UtcNow;
        public string SchemaVersion { get; set; }
    }
}
