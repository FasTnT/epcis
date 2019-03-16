using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class CustomFieldEntity : CustomField
    {
        public Guid EventId { get; set; }
    }
}
