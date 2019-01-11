using FasTnT.Model.Events.Enums;
using System;

namespace FasTnT.Model
{
    public class SourceDestination
    {
        public Guid EventId { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public SourceDestinationType Direction { get; set; }
    }
}
