using FasTnT.Model.Events;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class CustomFieldDto : EventRelatedDto
    {
        public short Id { get; set; }
        public short? ParentId { get; set; }

        internal CustomField ToCustomField()
        {
            return new CustomField(); // TODO
        }

        internal static CustomFieldDto Create(CustomField customField, short eventId, int requestId)
        {
            throw new NotImplementedException();
        }
    }
}
