using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FasTnT.Model;
using FasTnT.Model.Queries.Enums;
using FasTnT.Model.Events.Enums;
using Dapper;
using static Dapper.SqlBuilder;
using FasTnT.Domain.Persistence;
using MoreLinq;
using System.Threading;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlEventRepository : IEventRepository
    {
        private readonly DapperUnitOfWork _unitOfWork;
        private readonly QueryParameters _parameters = new QueryParameters();
        private readonly Template _sqlTemplate;
        private SqlBuilder _query = new SqlBuilder();

        private int _limit = 0;

        public PgSqlEventRepository(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sqlTemplate = _query.AddTemplate(SqlRequests.EventQuery);
        }

        public async Task<IEnumerable<EpcisEvent>> ToList(CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var events = await _unitOfWork.Query<EpcisEventEntity, ErrorDeclarationEntity>(_sqlTemplate.RawSql, _parameters.Values, (evt, ed) => evt.ErrorDeclaration = ed, "declaration_time", cancellationToken);

            using (var reader = await _unitOfWork.FetchMany(SqlRequests.RelatedQuery, new { EventIds = events.Select(x => x.Id).ToArray() }, cancellationToken))
            {
                var epcs = await reader.ReadAsync<EpcEntity>();
                var fields = await reader.ReadAsync<CustomFieldEntity>();
                var transactions = await reader.ReadAsync<BusinessTransactionEntity>();
                var sourceDests = await reader.ReadAsync<SourceDestinationEntity>();
                var correctiveEventIds = await reader.ReadAsync<CorrectiveEventIdEntity>();

                foreach (var evt in events)
                {
                    evt.Epcs = epcs.Where(x => x.EventId == evt.Id).ToList<Epc>();
                    evt.CustomFields = CreateHierarchy(fields.Where(x => x.EventId == evt.Id));
                    evt.BusinessTransactions = transactions.Where(x => x.EventId == evt.Id).ToList<BusinessTransaction>();
                    evt.SourceDestinationList = sourceDests.Where(x => x.EventId == evt.Id).ToList<SourceDestination>();

                    if (evt.ErrorDeclaration != null)
                    {
                        evt.ErrorDeclaration.CorrectiveEventIds = correctiveEventIds.Where(x => x.EventId == evt.Id).ToList<CorrectiveEventId>();
                    }
                }
            }

            return events;
        }

        private List<CustomField> CreateHierarchy(IEnumerable<CustomFieldEntity> customFields, int? parentId = null)
        {
            var elements = customFields.Where(x => x.ParentId == parentId);
            elements.ForEach(x => x.Children = CreateHierarchy(customFields, x.Id));

            return elements.ToList<CustomField>();
        }

        public void SetLimit(int eventLimit) => _limit = eventLimit;
        public void WhereSimpleFieldIn<T>(EpcisField field, T[] values) => _query = _query.Where($"{field.ToPgSql()} = ANY({_parameters.Add(values)})");
        public void WhereSimpleFieldMatches(EpcisField field, FilterComparator filterOperator, object value) => _query = _query.Where($"{field.ToPgSql()} {filterOperator.ToSql()} {_parameters.Add(value)}");

        public void WhereBusinessTransactionValueIn(string txName, string[] txValues)
            => _query = _query.Where($"EXISTS(SELECT bt.event_id FROM epcis.business_transaction bt WHERE bt.event_id = event.id AND bt.transaction_type = {_parameters.Add(txName)} AND bt.transaction_id = ANY({_parameters.Add(txValues)}))");

        public void WhereSourceDestinationValueIn(string sourceName, SourceDestinationType type, string[] sourceValues)
            => _query = _query.Where($"EXISTS(SELECT sd.event_id FROM epcis.source_destination sd WHERE sd.direction = {type.Id} AND sd.type = {_parameters.Add(sourceName)} AND sd.source_dest_id = ANY({_parameters.Add(sourceValues)}) AND sd.event_id = event.id)");

        public void WhereEpcMatches(string[] values, EpcType[] epcTypes)
            => _query = _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.event_id = event.id AND epc.epc LIKE ANY({_parameters.Add(values)}) AND epc.type = ANY({_parameters.Add(epcTypes)}))");

        public void WhereExistsErrorDeclaration()
            => _query = _query.Where($"EXISTS(SELECT ed.event_id FROM epcis.event_declaration ed WHERE ed.event_id = event.id)");

        public void WhereErrorReasonIn(string[] errorReasons)
            => _query = _query.Where($"EXISTS(SELECT ed.event_id FROM epcis.event_declaration ed WHERE ed.reason = ANY({_parameters.Add(errorReasons)}) AND ed.event_id = event.id)");

        public void WhereCorrectiveEventIdIn(string[] correctiveEventIds)
            => _query = _query.Where($"EXISTS(SELECT edi.event_id FROM epcis.event_declaration_eventid edi WHERE edi.corrective_eventid = ANY({_parameters.Add(correctiveEventIds)}) AND edi.event_id = event.id)");

        public void WhereCustomFieldMatches(bool inner, FieldType type, string fieldNamespace, string fieldName, string[] values) => throw new NotImplementedException();
        public void WhereCustomFieldMatches(bool inner, FieldType type, string fieldNamespace, string fieldName, FilterComparator comparator, object value) => throw new NotImplementedException();

        public void WhereCustomFieldExists(bool inner, FieldType fieldType, string fieldNamespace, string fieldName)
            => _query = _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = {fieldType.Id} AND cf.namespace = {_parameters.Add(fieldNamespace)} AND cf.name = {_parameters.Add(fieldName)} AND cf.parent_id IS {(inner ? "NOT" : "")} NULL)");
        
        public void WhereEpcQuantityMatches(FilterComparator filterOperator, double value)
            => _query = _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.type = {EpcType.Quantity} AND epc.quantity {filterOperator.ToSql()} {_parameters.Add(value)} AND epc.event_id = event.id)");

        public void OrderBy(EpcisField field, OrderDirection direction) => _query.OrderBy($"{field.ToPgSql()} {direction.ToPgSql()}");

        public void WhereMasterDataHierarchyContains(EpcisField field, string[] values)
            => _query = _query.Where($"({field.ToPgSql()} = ANY({_parameters.Add(values)}) OR EXISTS(SELECT h.parent_id FROM cbv.masterdata_hierarchy h WHERE h.parent_id = ANY({_parameters.Last}) AND h.children_id = {field.ToPgSql()} AND h.type = '{field.ToCbvType()}'))");

        public void WhereMasterdataHasAttribute(EpcisField attribute, string[] values)
            => _query = _query.Where($"EXISTS(SELECT a.id FROM cbv.attribute a WHERE a.masterdata_type = {_parameters.Add(attribute.ToCbvType())} AND a.masterdata_id = {attribute.ToPgSql()} AND a.id = ANY({_parameters.Add(values)}))");

        public void WhereMasterdataAttributeValueIn(EpcisField attribute, string id, string[] values)
            => _query = _query.Where($"EXISTS(SELECT a.id FROM cbv.attribute a WHERE a.masterdata_type = {_parameters.Add(attribute.ToCbvType())} AND a.masterdata_id = {attribute.ToPgSql()} AND a.id = {_parameters.Add(id)} AND a.value = ANY({_parameters.Add(values)}))");
    }
}
