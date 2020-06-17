using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Utils;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class CustomFieldDto : EventRelatedDto
    {
        public short Id { get; set; }
        public short? ParentId { get; set; }
        public short Type { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
        public DateTime? DateValue { get; set; }

        internal CustomField ToCustomField()
        {
            return new CustomField
            {
                Type = Enumeration.GetById<FieldType>(Type),
                Name = Name,
                Namespace = Namespace,
                TextValue = TextValue,
                NumericValue = NumericValue,
                DateValue = DateValue
            };
        }

        internal static CustomFieldDto Create(CustomField field, short fieldId, short eventId, int requestId, short? parentId)
        {
            return new CustomFieldDto
            {
                RequestId = requestId,
                EventId = eventId,
                Id = fieldId,
                ParentId = parentId,
                Type = field.Type.Id,
                Name = field.Name,
                Namespace = field.Namespace,
                TextValue = field.TextValue,
                NumericValue = field.NumericValue,
                DateValue = field.DateValue
            };
        }
    }
}
