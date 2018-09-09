namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class ExtensionFieldParameter : SimpleEventQueryParameter
    {
        public bool IsInner => Name.Contains("_INNER_");

        public string Namespace { get; set; }
    }
}
