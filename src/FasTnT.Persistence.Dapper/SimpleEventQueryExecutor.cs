using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries.Implementations.PredefinedQueries;
using FasTnT.Model.Queries.PredefinedQueries.Parameters;
using FasTnT.Model.Queries.PredefinedQueries.Parameters.Enums;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using MoreLinq;
using static Dapper.SqlBuilder;

// Review by LAA: remove magic strings
namespace FasTnT.Persistence.Dapper
{
    public class SimpleEventQueryExecutor : ISimpleEventQueryExecutor
    {
        private readonly IUnitOfWork _unitOfWork;
        private SqlBuilder _query;
        private Template _sqlTemplate;
        private QueryParameters _parameters;

        private int _limit = 0;
        private string _orderProperty = "request.record_time";
        private OrderDirection _orderDirection = OrderDirection.Desc; // Spec EPCIS 1.2, p68: default is DESC.

        public SimpleEventQueryExecutor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _query = new SqlBuilder();
            _parameters = new QueryParameters();
            _sqlTemplate = _query.AddTemplate(SqlRequests.EventQuery);
        }

        public void Apply(EventTypeParameter param) => _query.Where($"event_type = ANY({_parameters.Add(param.EventTypes)})");
        public void Apply(EventTimeParameter param) => _query.Where($"event.record_time {param.Comparator.ToSql()} {_parameters.Add(param.DateValue)}");
        public void Apply(RecordTimeParameter param) => _query.Where($"request.record_time {param.Comparator.ToSql()} {_parameters.Add(param.DateValue)}");
        public void Apply(ActionParameter param) => _query.Where($"action = ANY({_parameters.Add(param.Actions)})");
        public void Apply(BizStepParameter param) => _query.Where($"business_step = ANY({_parameters.Add(param.Values)})");
        public void Apply(DispositionParameter param) => _query.Where($"disposition = ANY({_parameters.Add(param.Values)})");
        public void Apply(ReadPointParameter param) => _query.Where($"read_point = ANY({_parameters.Add(param.Values)})");
        public void Apply(BizLocationParameter param) => _query.Where($"business_location = ANY({_parameters.Add(param.Values)})");
        public void Apply(TransformationParameter param) => _query.Where($"transformation_id = ANY({_parameters.Add(param.Values)})");
        public void Apply(BizTransactionParameter param) => _query.Where($"EXISTS(SELECT id FROM epcis.business_transaction WHERE type = ANY({_parameters.Add(param.Values)}) AND event_id = event.id)");
        public void Apply(QuantityParameter param) => _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.type = {EpcType.Quantity} AND epc.event_id = event.id AND epc.quantity = {_parameters.Add(param.NumericValue)})");
        public void Apply(MatchAnyEpcParameter param) => _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.epc = ANY({_parameters.Add(param.Values)}) AND epc.event_id = event.id)");
        public void Apply(MatchEpcParameter param) => _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.epc = ANY({_parameters.Add(param.Values)}) AND epc.type = {_parameters.Add(param.Type)} AND epc.event_id = event.id)");
        public void Apply(EventIdParameter param) => _query.Where($"event.custom_id = ANY({_parameters.Add(param.Values)})"); // TODO: add 'custom_id' field in the DB.
        public void Apply(ExtensionFieldParameter param) => _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = 1 AND cf.namespace = {_parameters.Add(param.Namespace)} AND cf.name = {_parameters.Add(param.Name)} AND cf.text_value = ANY({_parameters.Add(param.Values)}) AND cf.parent_id IS {(param.IsInner ? "NOT" : "")} NULL)");
        public void Apply(SourceDestinationParameter param) => _query.Where($"EXISTS(SELECT sd.event_id FROM epcis.source_destination sd WHERE sd.direction = AND sd.type = {_parameters.Add(param.Type)} AND st.source_dest_id = ANY({_parameters.Add(param.Values)}) AND sd.event_id = event.id)");
        public void Apply(OrderByParameter param) => _orderProperty = GetOrderFieldName(param.Value);
        public void Apply(OrderDirectionParameter param) => _orderDirection = param.Direction;
        public void Apply(EventCountLimitParameter param) => SetLimit(param.Limit);
        public void Apply(MaxEventCountParameter param) => SetLimit(param.Limit + 1);
        public void Apply(ExistsIlmdParameter param) => _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = 0 AND cf.namespace = {_parameters.Add(param.Namespace)} AND cf.name = {_parameters.Add(param.Property)} AND cf.parent_id IS {(param.IsInner ? "NOT" : "")} NULL)");
        public void Apply(IlmdParameter param)
        {
            if (param.ValueType == ParameterValueType.Date)
                _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = 0 AND cf.namespace = {_parameters.Add(param.Namespace)} AND cf.name = {_parameters.Add(param.Property)} AND cf.date_value {param.Comparator.ToSql()} {_parameters.Add(param.DateValue)} AND cf.parent_id IS {(param.IsInner ? "NOT" : "")} NULL)");
            else if (param.ValueType == ParameterValueType.Numeric)
                _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = 0 AND cf.namespace = {_parameters.Add(param.Namespace)} AND cf.name = {_parameters.Add(param.Property)} AND cf.numeric_value {param.Comparator.ToSql()} {_parameters.Add(param.NumericValue)} AND cf.parent_id IS {(param.IsInner ? "NOT" : "")} NULL)");
            else
                _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = 0 AND cf.namespace = {_parameters.Add(param.Namespace)} AND cf.name = {_parameters.Add(param.Property)} AND cf.text_value = ANY({_parameters.Add(param.Values)}) AND cf.parent_id IS {(param.IsInner ? "NOT" : "")} NULL)");
        }
        // TODO: other parameters: ErrorDeclarationParameter

        public void Apply(SimpleEventQueryParameter param) => throw new EpcisException(ExceptionType.ImplementationException, $"Parameter '{param.GetType().Name}' is not implemented yet.");

        public async Task<IEnumerable<EpcisEvent>> Execute(SimpleEventQuery query)
        {
            query.Parameters.ForEach(x => Apply((dynamic)x));
            _query.OrderBy($"{_orderProperty} {_orderDirection.ToSql()}"); // Apply order by parameter (default is request record_time desc).
            _parameters.SetLimit(_limit == 0 ? 1024 : _limit);

            var events = await _unitOfWork.Query<EpcisEvent>(_sqlTemplate.RawSql, _parameters.Values);

            // If a MaxEventCountParameter exist and the number of events == the limit, throw an exception
            EnsureMaxEventCountLimitIsRespected(query, events);

            using (var reader = await _unitOfWork.FetchMany(SqlRequests.RelatedQuery, new { EventIds = events.Select(x => x.Id).ToArray() }))
            {
                var epcs = await reader.ReadAsync<Epc>();
                var fields = await reader.ReadAsync<CustomField>();
                var transactions = await reader.ReadAsync<BusinessTransaction>();
                var sourceDests = await reader.ReadAsync<SourceDestination>();

                foreach (var evt in events)
                {
                    evt.Epcs = epcs.Where(x => x.EventId == evt.Id).ToList();
                    evt.CustomFields = fields.Where(x => x.EventId == evt.Id).ToList();
                    evt.BusinessTransactions = transactions.Where(x => x.EventId == evt.Id).ToList();
                    evt.SourceDestinationList = sourceDests.Where(x => x.EventId == evt.Id).ToList();
                }
            }

            return events;
        }

        private void EnsureMaxEventCountLimitIsRespected(SimpleEventQuery query, IEnumerable<EpcisEvent> events)
        {
            if (query.Parameters.SingleOrDefault(x => x is MaxEventCountParameter) is MaxEventCountParameter maxEventCountParameter && events.Count() == _limit)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned more than {maxEventCountParameter.Limit} events.");
            }
        }

        private string GetOrderFieldName(string value)
        {
            switch (value)
            {
                case "eventTime":
                    return "event.record_time";
                case "recordTime":
                    return "request.record_time";
                default:
                    throw new ArgumentException($"Property '{value}' is unexpected for order parameter");
            }
        }

        private void SetLimit(int limit)
        {
            if (_limit > 0) throw new EpcisException(ExceptionType.QueryParameterException, "MaxEventCount and EventCountLimit are mutually exclusive");
            _limit = limit;
        }
    }
}
