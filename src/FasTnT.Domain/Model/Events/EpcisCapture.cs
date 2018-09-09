using System;

namespace FasTnT.Domain
{
    public abstract class EpcisCapture
    {
        public DateTime CreationDate { get; set; }
        public string SchemaVersion { get; set; }
    }
}
