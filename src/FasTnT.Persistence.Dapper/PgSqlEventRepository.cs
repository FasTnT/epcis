using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain;
using FasTnT.Model.Exceptions;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Model.Queries.Enums;
using static Dapper.SqlBuilder;

// Review by LAA: remove magic strings
namespace FasTnT.Persistence.Dapper
{
    public class PgSqlEventRepository : IEventRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private SqlBuilder _query;
        private Template _sqlTemplate;
        private QueryParameters _parameters;

        private int _limit = 0;

        public PgSqlEventRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _query = new SqlBuilder();
            _parameters = new QueryParameters();
            _sqlTemplate = _query.AddTemplate(SqlRequests.EventQuery);
        }

        public async Task<IEnumerable<EpcisEvent>> ToList()
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var events = await _unitOfWork.Query<EpcisEvent>(_sqlTemplate.RawSql, _parameters.Values);

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

        public void SetEventLimit(int eventLimit)
        {
            if (_limit > 0) throw new EpcisException(ExceptionType.QueryParameterException, "MaxEventCount and EventCountLimit are mutually exclusive");
            _limit = eventLimit;
        }


        public void SetMaxEventCount(int maxEventCount)
        {
            if (_limit > 0) throw new EpcisException(ExceptionType.QueryParameterException, "MaxEventCount and EventCountLimit are mutually exclusive");
            _limit = maxEventCount + 1;
        }

        public void WhereRequestIdIn(params Guid[] requestIds)
            => _query = _query.Where($"request.id = ANY({_parameters.Add(requestIds)})");

        public void WhereBusinessLocationIn(params string[] businessLocations)
            => _query = _query.Where($"event.business_location = ANY({_parameters.Add(businessLocations)})");

        public void WhereEventIdIn(params string[] eventIds)
            => _query = _query.Where($"event.event_id = ANY({_parameters.Add(eventIds)})");

        public void WhereActionIn(params EventAction[] actions)
            => _query = _query.Where($"event.action = ANY({_parameters.Add(actions)})");

        public void WhereBusinessStepIn(params string[] businessSteps)
            => _query = _query.Where($"event.business_step = ANY({_parameters.Add(businessSteps)})");

        public void WhereDispositionIn(params string[] dispositions)
            => _query = _query.Where($"event.disposition = ANY({_parameters.Add(dispositions)})");

        public void WhereReadPointIn(params string[] readPoints)
            => _query = _query.Where($"event.read_point = ANY({_parameters.Add(readPoints)})");

        public void WhereBusinessTransactionValueIn(string txName, params string[] txValues)
            => throw new NotImplementedException();

        public void WhereSourceValueIn(string sourceName, params string[] sourceValues)
            => _query = _query.Where($"EXISTS(SELECT sd.event_id FROM epcis.source_destination sd WHERE sd.direction = {SourceDestinationType.Source.Id} AND sd.type = {_parameters.Add(sourceName)} AND sd.source_dest_id = ANY({_parameters.Add(sourceValues)}) AND sd.event_id = event.id)");

        public void WhereDestinationValueIn(string destName, params string[] destValues)
            => throw new NotImplementedException();

        public void WhereTransformationIdIn(params string[] transformationIds)
            => _query = _query.Where($"event.transformation_id = ANY({_parameters.Add(transformationIds)})");

        public void WhereEpcMatches(string[] values, EpcType epcType = null)
            => throw new NotImplementedException();

        public void WhereExistsErrorDeclaration()
            => throw new NotImplementedException();

        public void WhereErrorReasonIn(params string[] errorReasons)
            => throw new NotImplementedException();

        public void WhereCorrectiveEventIdIn(params string[] correctiveEventIds)
            => throw new NotImplementedException();

        public void WhereMatchesIlmd<T>(bool inner, string ilmdNamespace, string ilmdName, string comparator, T values)
            => throw new NotImplementedException();

        public void WhereExistsIlmd(bool inner, string ilmdNamespace, string ilmdName)
            => _query = _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = {FieldType.Ilmd.Id} AND cf.namespace = {_parameters.Add(ilmdNamespace)} AND cf.name = {_parameters.Add(ilmdName)} AND cf.parent_id IS {(inner ? "NOT" : "")} NULL)");

        public void WhereMatchesCustomField<T>(bool inner, string fieldNamespace, string fieldName, string comparator, T values)
            => throw new NotImplementedException();

        public void WhereExistsCustomField(bool inner, string fieldNamespace, string fieldName)
            => _query = _query.Where($"EXISTS(SELECT cf.event_id FROM epcis.custom_field cf WHERE cf.event_id = event.id AND cf.type = {FieldType.EventExtension.Id} AND cf.namespace = {_parameters.Add(fieldNamespace)} AND cf.name = {_parameters.Add(fieldName)} AND cf.parent_id IS {(inner ? "NOT" : "")} NULL)");

        public void WhereCaptureTimeMatches(FilterOperator filterOperator, DateTime dateTime)
            => _query = _query.Where($"event.record_time {filterOperator.ToSql()} {_parameters.Add(dateTime)}");

        public void WhereRecordTimeMatches(FilterOperator filterOperator, DateTime dateTime)
            => _query = _query.Where($"request.record_time {filterOperator.ToSql()} {_parameters.Add(dateTime)}");

        public void WhereEventTypeIn(string[] values)
            => _query = _query.Where($"event.event_type = ANY({_parameters.Add(values)})");

        public void WhereEpcQuantityMatches(FilterOperator filterOperator, double value)
            => _query = _query.Where($"EXISTS(SELECT epc.event_id FROM epcis.epc epc WHERE epc.type = {EpcType.Quantity} AND epc.quantity {filterOperator.ToSql()} {_parameters.Add(value)} AND epc.event_id = event.id)");
    }
}
