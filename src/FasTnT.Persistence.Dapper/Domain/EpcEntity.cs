using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class EpcEntity : Epc
    {
        public Guid EventId { get; set; }
    }
}
