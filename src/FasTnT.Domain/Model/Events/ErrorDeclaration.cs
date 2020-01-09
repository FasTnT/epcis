using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class ErrorDeclaration
    {
        public DateTime DeclarationTime { get; set; }
        public string Reason { get; set; }
        public IList<CorrectiveEventId> CorrectiveEventIds { get; set; } = new List<CorrectiveEventId>();
    }
}
