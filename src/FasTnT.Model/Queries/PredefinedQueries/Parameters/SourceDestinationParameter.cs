using FasTnT.Domain;
using System.Linq;

namespace FasTnT.Model.Queries.PredefinedQueries.Parameters
{
    public class SourceDestinationParameter : SimpleEventQueryParameter
    {
        public SourceDestinationType Direction { get; set; }
        public string Type => Name.Split('_', 3).Last();

    }
}
