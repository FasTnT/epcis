using FasTnT.Model.Enums;

namespace FasTnT.Model.Events
{
    public class SourceDestination
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public SourceDestinationType Direction { get; set; }
    }
}
