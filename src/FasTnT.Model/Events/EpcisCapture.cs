using System;

namespace FasTnT.Domain
{
    public abstract class Request
    {
        public DateTime CreationDate { get; set; }
        public string SchemaVersion { get; set; }
    }
}
