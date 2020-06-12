namespace FasTnT.Data.PostgreSql.DTOs
{
    public class CorrectiveIdDto : EventRelatedDto
    {
        public string Id { get; set; }

        internal string ToCorrectiveId()
        {
            return Id;
        }

        internal static CorrectiveIdDto Create(string correctiveId, short eventId, int requestId)
        {
            return new CorrectiveIdDto
            {
                EventId = eventId,
                RequestId = requestId,
                Id = correctiveId
            };
        }
    }
}
