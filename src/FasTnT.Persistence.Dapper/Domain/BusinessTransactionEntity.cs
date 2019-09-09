using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class BusinessTransactionEntity : BusinessTransaction
    {
        public int EventId { get; set; }
    }
}
