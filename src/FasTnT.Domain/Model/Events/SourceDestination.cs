using System;

namespace FasTnT.Domain
{
    public class SourceDestination
    {
        public Guid EventId { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public SourceDestinationType Direction { get; set; }
    }
}
