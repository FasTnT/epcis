using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class ErrorDeclarationEntity : ErrorDeclaration
    {
        public Guid EventId { get; set; }
    }
}
