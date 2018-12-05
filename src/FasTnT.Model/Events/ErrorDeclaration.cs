using System;
using System.Collections.Generic;

namespace FasTnT.Domain
{
    public class ErrorDeclaration
    {
        public Guid EventId { get; set; }
        public DateTime DeclarationTime { get; set; }
        public string Reason { get; set; }
        public IList<CorrectiveEventId> CorrectiveEventIds { get; set; } = new List<CorrectiveEventId>();
    }

    public class CorrectiveEventId
    {
        public Guid EventId { get; set; }
        public string CorrectiveId { get; set; }
    }
}
