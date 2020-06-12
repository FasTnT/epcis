using FasTnT.Model.Events;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class TransactionDto : EventRelatedDto
    {
        public string Type { get; set; }
        public string Id { get; set; }

        internal BusinessTransaction ToBusinessTransaction()
        {
            return new BusinessTransaction
            {
                Id = Id,
                Type = Type
            };
        }

        internal static TransactionDto Create(BusinessTransaction tx, short eventId, int requestId)
        {
            return new TransactionDto
            {
                EventId = eventId,
                RequestId = requestId,
                Id = tx.Id,
                Type = tx.Type
            };
        }
    }
}
