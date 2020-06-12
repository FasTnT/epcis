using System;
using System.Collections.Generic;

namespace FasTnT.Model.Events
{
    public class ErrorDeclaration
    {
        public DateTime? DeclarationTime { get; set; }
        public string Reason { get; set; }
    }
}
