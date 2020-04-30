using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public class QueryFilters
    {
        internal const string Epcs = "epcis.epc";
        internal const string BusinessTransactions = "epcis.business_transaction";
        internal const string ErrorDeclaration = "epcis.event_declaration";
        internal const string CustomFields = "epcis.custom_field";

        private IDictionary<string, IList<string>> _filters = new Dictionary<string, IList<string>>
        {
            { Epcs, new List<string>() },
            { BusinessTransactions, new List<string>() },
            { ErrorDeclaration, new List<string>() },
            { CustomFields, new List<string>() },
        };

        public bool ContainsFilters => _filters.Any(f => f.Value.Any());
        public void AddCondition(string key, string condition) => _filters[key].Add(condition);

        public string GetSqlFilters()
        {
            var sqlFilters = _filters
                .Where(f => f.Value.Any())
                .Select(f => $"EXISTS(SELECT 1 FROM {f.Key} WHERE event_id = event.id AND {string.Join(" AND ", f.Value.Select(x => $"({x})"))})");

            return sqlFilters.Any() ? string.Join(" AND ", sqlFilters) : null;
        }
    }
}
