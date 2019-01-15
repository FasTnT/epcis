using System;

namespace FasTnT.Model
{
    public class BusinessTransaction
    {
        public Guid EventId { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
    }
}
