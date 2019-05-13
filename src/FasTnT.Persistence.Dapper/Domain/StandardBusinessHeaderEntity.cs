using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class StandardBusinessHeaderEntity : StandardBusinessHeader
    {
        public Guid Id { get; set; }
    }
}
