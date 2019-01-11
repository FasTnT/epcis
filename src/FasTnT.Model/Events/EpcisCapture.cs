using System;

namespace FasTnT.Model
{
    public abstract class Request
    {
        public DateTime CreationDate { get; set; }
        public string SchemaVersion { get; set; } = "1";
    }
}
