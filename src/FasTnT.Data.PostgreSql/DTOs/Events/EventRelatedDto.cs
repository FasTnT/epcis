namespace FasTnT.Data.PostgreSql.DTOs
{
    public abstract class EventRelatedDto
    {
        public int RequestId { get; set; }
        public short EventId { get; set; }

        internal bool Matches(EventDto eventDto)
        {
            return eventDto.Id == EventId && eventDto.RequestId == RequestId;
        }
    }
}
