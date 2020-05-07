using Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlBuilder;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public class EventFetcher : IEventFetcher
    {
        private readonly QueryParameters _parameters = new QueryParameters();
        private readonly QueryFilters _filters = new QueryFilters();
        private readonly Template _sqlTemplate;
        private SqlBuilder _query = new SqlBuilder();
        private int? _limit;
        private readonly IDbConnection _connection;

        public EventFetcher(IDbConnection connection)
        {
            _connection = connection;
            _sqlTemplate = _query.AddTemplate(PgSqlEventRequests.EventQuery);
        }

        public void Apply(RequestIdFilter filter) => _query = _query.Where($"request.id = ANY({_parameters.Add(filter.Values)})");
        public void Apply<T>(SimpleParameterFilter<T> filter) => _query = _query.Where($"{filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonParameterFilter filter) => _query = _query.Where($"{filter.Field.ToPgSql()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(BusinessTransactionFilter filter) => _filters.AddCondition(QueryFilters.BusinessTransactions, $"transaction_type = {_parameters.Add(filter.TransactionType)} AND transaction_id = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MatchEpcFilter filter) => _filters.AddCondition(QueryFilters.Epcs, $"epc LIKE ANY({_parameters.Add(filter.Values)}) AND type = ANY({_parameters.Add(filter.EpcType)})");
        public void Apply(QuantityFilter filter) => _filters.AddCondition(QueryFilters.Epcs, $"type = {EpcType.Quantity} AND quantity {filter.Operator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(ExistCustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL");
        public void Apply(ExistsErrorDeclarationFilter filter) => _filters.AddCondition(QueryFilters.ErrorDeclaration, "event_id IS NOT NULL");
        public void Apply(EqualsErrorReasonFilter filter) => _filters.AddCondition(QueryFilters.ErrorDeclaration, $"ed.reason = ANY({_parameters.Add(filter.Values)})");
        public void Apply(EqualsCorrectiveEventIdFilter filter) => _query = _query.Where($"EXISTS(SELECT edi.event_id FROM epcis.event_declaration_eventid edi WHERE edi.corrective_eventid = ANY({_parameters.Add(filter.Values)}) AND edi.event_id = event.id)");
        public void Apply(MasterdataHierarchyFilter filter) => _query = _query.Where($"({filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)}) OR EXISTS(SELECT h.parent_id FROM cbv.masterdata_hierarchy h WHERE h.parent_id = ANY({_parameters.Last}) AND h.children_id = {filter.Field.ToPgSql()} AND h.type = '{filter.Field.ToCbvType()}'))");
        public void Apply(SourceDestinationFilter filter) => _query = _query.Where($"EXISTS(SELECT sd.event_id FROM epcis.source_destination sd WHERE sd.direction = {filter.Type.Id} AND sd.type = {_parameters.Add(filter.Name)} AND sd.source_dest_id = ANY({_parameters.Add(filter.Values)}) AND sd.event_id = event.id)");
        public void Apply(ExistsAttributeFilter filter) => _query = _query.Where($"EXISTS(SELECT at.id FROM cbv.attribute at WHERE at.masterdata_id = {filter.Field.ToPgSql()} AND at.id = {_parameters.Add(filter.AttributeName)})");
        public void Apply(AttributeFilter filter) => _query = _query.Where($"EXISTS(SELECT at.id FROM cbv.attribute at WHERE at.masterdata_id = {filter.Field.ToPgSql()} AND at.id = {_parameters.Add(filter.AttributeName)} AND at.value = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(CustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL AND text_value = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonCustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL AND {filter.Value.GetCustomFieldName()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(LimitFilter filter) => _limit = filter.Value;

        public async Task<IEnumerable<EpcisEvent>> Fetch(CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit ?? int.MaxValue);
            _query = _filters.ContainsFilters ? _query.Where(_filters.GetSqlFilters()) : _query;

            using (var reader = await _connection.QueryMultipleAsync(new CommandDefinition(_sqlTemplate.RawSql, _parameters.Values, cancellationToken: cancellationToken)))
            {
                var events = await reader.ReadAsync<EpcisEvent>();
                var errorDeclarations = await reader.ReadAsync<ErrorDeclaration>();
                var epcs = await reader.ReadAsync<Epc>();
                var fields = await reader.ReadAsync<CustomField>();
                var transactions = await reader.ReadAsync<BusinessTransaction>();
                var sourceDests = await reader.ReadAsync<SourceDestination>();
                var correctiveEventIds = await reader.ReadAsync<CorrectiveEventId>();

                errorDeclarations.ForEach(err => err.CorrectiveEventIds = correctiveEventIds.Where(x => x.EventId == err.EventId).ToList());
                events.ForEach(evt =>
                {
                    evt.Epcs = epcs.Where(x => x.EventId == evt.Id).ToList();
                    evt.CustomFields = CreateHierarchy(fields.Where(x => x.EventId == evt.Id));
                    evt.BusinessTransactions = transactions.Where(x => x.EventId == evt.Id).ToList();
                    evt.SourceDestinationList = sourceDests.Where(x => x.EventId == evt.Id).ToList();
                    evt.ErrorDeclaration = errorDeclarations.FirstOrDefault(x => x.EventId == evt.Id);
                });

                return events;
            }
        }

        private List<CustomField> CreateHierarchy(IEnumerable<CustomField> customFields, int? parentId = null)
        {
            var elements = customFields.Where(x => x.ParentId == parentId);

            foreach (var element in elements)
            {
                element.Children = CreateHierarchy(customFields, element.Id);
            }

            return elements.ToList();
        }
    }
}
