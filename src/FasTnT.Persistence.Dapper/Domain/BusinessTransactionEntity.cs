using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class BusinessTransactionEntity : BusinessTransaction
    {
        public Guid EventId { get; set; }
    }
}
