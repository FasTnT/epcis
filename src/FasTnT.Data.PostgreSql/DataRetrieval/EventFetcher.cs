using Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
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

        public void Apply(SimpleParameterFilter filter) => _query = _query.Where($"{filter.Field.ToPgSql()} = ANY({_parameters.Add(filter.Values)})");
        public void Apply(ComparisonParameterFilter filter) => _query = _query.Where($"{filter.Field.ToPgSql()} {filter.Comparator.ToSql()} {_parameters.Add(filter.Value)}");
        public void Apply(BusinessTransactionFilter filter) => throw new System.NotImplementedException();
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
