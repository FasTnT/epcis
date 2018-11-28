using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class IlmdParameter : SimpleEventQueryParameter
    {
        public string Namespace => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[0];
        public string Property => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[1];
        public bool IsInner { get; set; }
    }
}
