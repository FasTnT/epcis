using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class EpcisEventEntity : EpcisEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequestId { get; set; }
    }
}
