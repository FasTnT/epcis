using FasTnT.Model.Enums;
using FasTnT.Model.Events;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class SourceDestDto : EventRelatedDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public SourceDestinationType Direction { get; set; }

        internal SourceDestination ToSourceDestination()
        {
            return new SourceDestination
            {
                Id = Id,
                Type = Type,
                Direction = Direction
            };
        }

        internal static SourceDestDto Create(SourceDestination sd, short eventId, int requestId)
        {
            return new SourceDestDto
            {
                EventId = eventId,
                RequestId = requestId,
                Type = sd.Type,
                Id = sd.Id,
                Direction = sd.Direction
            };
        }
    }
}
