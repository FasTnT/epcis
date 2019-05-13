using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class SourceDestinationType : Enumeration
    {
        public static SourceDestinationType Source = new SourceDestinationType(0, "source");
        public static SourceDestinationType Destination = new SourceDestinationType(1, "destination");

        public SourceDestinationType() { }
        public SourceDestinationType(short id, string displayName) : base(id, displayName) { }
    }
}
