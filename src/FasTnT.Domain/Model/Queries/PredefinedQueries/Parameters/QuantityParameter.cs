namespace FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters
{
    public class QuantityParameter : SimpleEventQueryParameter
    {
        public int DecimalValue => int.Parse(Value);
    }
}
