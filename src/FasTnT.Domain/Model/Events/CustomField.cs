using FasTnT.Model.Events.Enums;
using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class CustomField
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public int? EventId { get; set; }
        public FieldType Type { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
        public DateTime? DateValue { get; set; }
        public IList<CustomField> Children { get; set; } = new List<CustomField>();
    }
}
