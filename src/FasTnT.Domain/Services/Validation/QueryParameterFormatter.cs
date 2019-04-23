using FasTnT.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Domain.Services
{
    public class QueryParameterFormatter
    {
        public static IEnumerable<QueryParameter> Format(IEnumerable<QueryParameter> parameters)
            => parameters.GroupBy(x => x.Name)
                         .Select(g => new QueryParameter
                         {
                             Name = g.Key,
                             Values = g.SelectMany(x => x.Values).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()
                         })
                         .Where(x => x.Values.Any());
    }
}
