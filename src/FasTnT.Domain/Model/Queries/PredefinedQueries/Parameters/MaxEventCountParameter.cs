namespace FasTnT.Domain.Model.Queries.PredefinedQueries.Parameters
{
    public class MaxEventCountParameter : SimpleEventQueryParameter
    {
        public int Limit => int.Parse(Value);
    }
}
