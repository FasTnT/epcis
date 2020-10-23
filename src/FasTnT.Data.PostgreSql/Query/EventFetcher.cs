using Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.PostgreSql.DapperConfiguration;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlBuilder;

namespace FasTnT.Data.PostgreSql.Query
{
    public class EventFetcher : IEventFetcher
    {
        private readonly QueryParameters _parameters = new QueryParameters();
        private readonly QueryFilters _filters = new QueryFilters();
        private readonly Template _sqlTemplate;
        private SqlBuilder _query = new SqlBuilder();
        private EpcisField _orderField = EpcisField.CaptureTime;
        private OrderDirection _orderDirection = OrderDirection.Ascending;
        private int? _limit;
        private readonly IDbConnection _connection;

        public EventFetcher(IDbConnection connection)
        {
            _connection = connection;
            _sqlTemplate = _query.AddTemplate(SqlQueries.Read_Events);
        }

        public void Apply(RequestIdFilter filter) => _query = _query.Where($"request.id = ANY({_parameters.Add(filter.Values)})");
        public void Apply<T>(SimpleParameterFilter<T> filter) => _query = _query.Where($"{filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonParameterFilter filter) => _query = _query.Where($"{filter.Field.ToPgSql()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(EqualsCorrectiveEventIdFilter filter) => _query = _query.Where($"EXISTS(SELECT edi.event_id FROM epcis.event_declaration_eventid edi WHERE edi.corrective_eventid = ANY({_parameters.Add(filter.Values)}) AND edi.event_id = event.id)");
        public void Apply(MasterdataHierarchyFilter filter) => _query = _query.Where($"({filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)}) OR EXISTS(SELECT h.parent_id FROM cbv.masterdata_hierarchy h WHERE h.parent_id = ANY({_parameters.Last}) AND h.children_id = {filter.Field.ToPgSql()} AND h.type = '{filter.Field.ToCbvType()}'))");
        public void Apply(BusinessTransactionFilter filter) => _filters.AddCondition(QueryFilters.BusinessTransactions, $"transaction_type = {_parameters.Add(filter.TransactionType)} AND transaction_id = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ExistsErrorDeclarationFilter filter) => _query = _query.Where("errordeclaration_time IS NOT NULL");
        public void Apply(EqualsErrorReasonFilter filter) => _query = _query.Where($"errordeclaration_reason = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MatchEpcFilter filter) => _filters.AddCondition(QueryFilters.Epcs, $"epc LIKE ANY({_parameters.Add(filter.Values)}) AND type = ANY({_parameters.Add(filter.EpcType)})");
        public void Apply(QuantityFilter filter) => _filters.AddCondition(QueryFilters.Epcs, $"type = {EpcType.Quantity.Id} AND quantity {filter.Operator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(ExistCustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL");
        public void Apply(SourceDestinationFilter filter) => _filters.AddCondition(QueryFilters.SourceDestination, $"direction = {filter.Type.Id} AND type = {_parameters.Add(filter.Name)} AND source_dest_id = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ExistsAttributeFilter filter) => _filters.AddCondition(QueryFilters.Cbv, $"masterdata_id = {filter.Field.ToPgSql()} AND id = {_parameters.Add(filter.AttributeName)}");
        public void Apply(AttributeFilter filter) => _filters.AddCondition(QueryFilters.Cbv, $"masterdata_id = {filter.Field.ToPgSql()} AND id = {_parameters.Add(filter.AttributeName)} AND value = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(CustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL AND text_value = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonCustomFieldFilter filter) => _filters.AddCondition(QueryFilters.CustomFields, $"type = {filter.Field.Type.Id} AND namespace = {_parameters.Add(filter.Field.Namespace)} AND name = {_parameters.Add(filter.Field.Name)} AND parent_id IS {(filter.IsInner ? "NOT" : "")} NULL AND {filter.Value.GetCustomFieldName()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(LimitFilter filter) => _limit = filter.Value;
        public void Apply(OrderFilter filter) => _orderField = filter.Field;
        public void Apply(OrderDirectionFilter filter) => _orderDirection = filter.Direction;

        public async Task<IEnumerable<EpcisEvent>> Fetch(CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit ?? int.MaxValue);
            _query = _filters.ContainsFilters ? _query.Where(_filters.GetSqlFilters()) : _query;
            _query = _query.OrderBy($"{_orderField.ToPgSql()} {_orderDirection.ToPgSql()}");

            using var reader = await _connection.QueryMultipleAsync(new CommandDefinition(_sqlTemplate.RawSql, _parameters.Values, cancellationToken: cancellationToken));
            var eventDtoManager = await EventDtoManager.ReadAsync(reader, cancellationToken);

            return eventDtoManager.FormatEvents();
        }
    }
}
