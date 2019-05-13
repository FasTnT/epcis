using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class CorrectiveEventIdEntity : CorrectiveEventId
    {
        public Guid EventId { get; set; }
    }
}
