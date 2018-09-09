using System;

namespace FasTnT.Domain
{
    public class CustomField
    {
        public Guid EventId { get; set; }
        public int Id { get; set; }
        public FieldType Type { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
        public DateTime? DateValue { get; set; }
        public int? ParentId { get; set; }
    }
}
