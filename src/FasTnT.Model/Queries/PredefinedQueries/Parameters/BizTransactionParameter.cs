using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class BizTransactionParameter : SimpleEventQueryParameter
    {
        public string TransactionType => Name.Split('_', 3).Last();
    }
}
