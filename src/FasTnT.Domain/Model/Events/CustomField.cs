using FasTnT.Model.Enums;
using System;
using System.Collections.Generic;

namespace FasTnT.Model.Events
{
    public class CustomField
    {
        public int? Id { get; set; }
        public int? HeaderId { get; set; }
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
