namespace FasTnT.Domain.Data.Model.Filters
{
    public class BusinessTransactionFilter
    {
        public string TransactionType { get; set; }
        public object[] Values { get; set; }
    }
}
