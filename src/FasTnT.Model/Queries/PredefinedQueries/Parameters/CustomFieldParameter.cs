using FasTnT.Domain;
using FasTnT.Model.Utils;
using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class CustomFieldParameter : SimpleEventQueryParameter
    {
        public bool IsInner { get; set; }
        public FieldType FieldType { get; set; }
        public string Namespace => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[0];
        public string Property => Name.Split('_').Single(x => x.Contains('#')).Split('#', 2)[1];
        public ParameterComparator Comparator => Enumeration.GetByDisplayName<ParameterComparator>(Name.Substring(0, 2));
    }
}
