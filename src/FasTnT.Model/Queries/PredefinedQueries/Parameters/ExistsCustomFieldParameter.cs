using FasTnT.Domain;
using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class ExistsCustomFieldParameter : SimpleEventQueryParameter
    {
        public bool IsInner { get; set; }
        public FieldType FieldType { get; set; }
        public string Namespace => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[0];
        public string Property => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[1];
    }
}
