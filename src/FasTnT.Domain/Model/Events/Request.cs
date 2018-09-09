using System;
using System.Collections.Generic;

namespace FasTnT.Domain.Model.Events
{
    public class Request
    {
        public DateTime RequestDate { get; set; }
        public string SchemaVersion { get; set; }
        public IEnumerable<EpcisEvent> Events { get; set; }
    }
}
