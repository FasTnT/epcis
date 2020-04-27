using System;
using System.Collections.Generic;

namespace FasTnT.Model.Events
{
    public class ErrorDeclaration
    {
        public int? EventId { get; set; }
        public DateTime DeclarationTime { get; set; }
        public string Reason { get; set; }
        public IList<CorrectiveEventId> CorrectiveEventIds { get; set; } = new List<CorrectiveEventId>();
    }
}
