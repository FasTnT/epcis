using Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
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
        private readonly Template _sqlTemplate;
        private SqlBuilder _query = new SqlBuilder();
        private int _limit;
        private readonly IDbConnection _connection;

        public EventFetcher(IDbConnection connection)
        {
            _connection = connection;
            _sqlTemplate = _query.AddTemplate(PgSqlEventRequests.EventQuery);
        }

        public void Apply(RequestIdFilter filter) => _query = _query.Where($"request.id = ANY({_parameters.Add(filter.Values)})");
        public void Apply<T>(SimpleParameterFilter<T> filter) => _query = _query.Where($"{filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonParameterFilter filter) => _query = _query.Where($"{filter.Field.ToPgSql()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(BusinessTransactionFilter filter) => _query = _query.Where($"EXISTS(SELECT bt.event_id FROM epcis.business_transaction bt WHERE bt.event_id = event.id AND bt.transaction_type = {_parameters.Add(filter.TransactionType)} AND bt.transaction_id = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(MatchEpcFilter filter) => _query = _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.event_id = event.id AND epc.epc LIKE ANY({_parameters.Add(filter.Values)}) AND epc.type = ANY({_parameters.Add(filter.EpcType)}))");
        public void Apply(QuantityFilter filter) => _query = _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.type = {EpcType.Quantity} AND epc.quantity {filter.Operator.ToSql()} {_parameters.Add(filter.Value)} AND epc.event_id = event.id)");
        public void Apply(ExistCustomFieldFilter filter) => _query = _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = {filter.Field.Type.Id} AND cf.namespace = {_parameters.Add(filter.Field.Namespace)} AND cf.name = {_parameters.Add(filter.Field.Name)} AND cf.parent_id IS {(filter.IsInner ? "NOT" : "")} NULL)");
        public void Apply(ExistsErrorDeclarationFilter filter) => _query = _query.Where($"EXISTS(SELECT ed.event_id FROM epcis.event_declaration ed WHERE ed.event_id = event.id)");
        public void Apply(EqualsErrorReasonFilter filter) => _query = _query.Where($"EXISTS(SELECT ed.event_id FROM epcis.event_declaration ed WHERE ed.reason = ANY({_parameters.Add(filter.Values)}) AND ed.event_id = event.id)");
        public void Apply(EqualsCorrectiveEventIdFilter filter) => _query = _query.Where($"EXISTS(SELECT edi.event_id FROM epcis.event_declaration_eventid edi WHERE edi.corrective_eventid = ANY({_parameters.Add(filter.Values)}) AND edi.event_id = event.id)");
        public void Apply(MasterdataHierarchyFilter filter) => _query = _query.Where($"({filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)}) OR EXISTS(SELECT h.parent_id FROM cbv.masterdata_hierarchy h WHERE h.parent_id = ANY({_parameters.Last}) AND h.children_id = {filter.Field.ToPgSql()} AND h.type = '{filter.Field.ToCbvType()}'))");
        public void Apply(SourceDestinationFilter filter) => _query = _query.Where($"EXISTS(SELECT sd.event_id FROM epcis.source_destination sd WHERE sd.direction = {filter.Type.Id} AND sd.type = {_parameters.Add(filter.Name)} AND sd.source_dest_id = ANY({_parameters.Add(filter.Values)}) AND sd.event_id = event.id)");
        public void Apply(ExistsAttributeFilter filter) => _query = _query.Where($"EXISTS(SELECT at.id FROM cbv.attribute at WHERE at.masterdata_id = {filter.Field.ToPgSql()} AND at.id = {_parameters.Add(filter.AttributeName)})");
        public void Apply(AttributeFilter filter) => _query = _query.Where($"EXISTS(SELECT at.id FROM cbv.attribute at WHERE at.masterdata_id = {filter.Field.ToPgSql()} AND at.id = {_parameters.Add(filter.AttributeName)} AND at.value = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(CustomFieldFilter filter) => throw new System.NotImplementedException();
        public void Apply(LimitFilter filter) => _limit = filter.Value;

        public async Task<IEnumerable<EpcisEvent>> Fetch(CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var events = await _connection.QueryAsync<EpcisEvent, ErrorDeclaration, EpcisEvent>(new CommandDefinition(_sqlTemplate.RawSql, _parameters.Values, cancellationToken: cancellationToken), (evt, err) => evt, "declaration_time");

            using (var reader = await _connection.QueryMultipleAsync(PgSqlEventRequests.RelatedQuery, new { EventIds = events.Select(x => x.Id.Value).ToArray() }))
            {
                var epcs = await reader.ReadAsync<Epc>();
                var fields = await reader.ReadAsync<CustomField>();
                var transactions = await reader.ReadAsync<BusinessTransaction>();
                var sourceDests = await reader.ReadAsync<SourceDestination>();
                var correctiveEventIds = await reader.ReadAsync<CorrectiveEventId>();

                foreach (var evt in events)
                {
                    evt.Epcs = epcs.Where(x => x.EventId == evt.Id).ToList();
                    evt.CustomFields = CreateHierarchy(fields.Where(x => x.EventId == evt.Id));
                    evt.BusinessTransactions = transactions.Where(x => x.EventId == evt.Id).ToList();
                    evt.SourceDestinationList = sourceDests.Where(x => x.EventId == evt.Id).ToList();

                    if (evt.ErrorDeclaration != null)
                    {
                        evt.ErrorDeclaration.CorrectiveEventIds = correctiveEventIds.Where(x => x.EventId == evt.Id).ToList();
                    }
                }
            }

            return events.Cast<EpcisEvent>();
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
