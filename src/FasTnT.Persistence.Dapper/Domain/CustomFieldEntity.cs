using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class CustomFieldEntity : CustomField
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public Guid EventId { get; set; }
    }
}
