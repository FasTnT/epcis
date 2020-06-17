using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Utils;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class SourceDestDto : EventRelatedDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public short Direction { get; set; }

        internal SourceDestination ToSourceDestination()
        {
            return new SourceDestination
            {
                Id = Id,
                Type = Type,
                Direction = Enumeration.GetById<SourceDestinationType>(Direction)
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
                Direction = sd.Direction.Id
            };
        }
    }
}
