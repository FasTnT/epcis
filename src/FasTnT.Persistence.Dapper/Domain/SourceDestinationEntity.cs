using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class SourceDestinationEntity : SourceDestination
    {
        public Guid EventId { get; set; }
    }
}
