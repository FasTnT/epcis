using Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.MasterDatas;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlBuilder;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public class MasterdataFetcher : IMasterdataFetcher
    {
        private readonly QueryParameters _parameters = new QueryParameters();
        private readonly Template _sqlTemplate;
        private SqlBuilder _query = new SqlBuilder();
        private int _limit;
        private readonly IDbConnection _connection;

        public MasterdataFetcher(IDbConnection connection)
        {
            _connection = connection;
            _sqlTemplate = _query.AddTemplate(PgSqlMasterdataRequests.MasterdataQuery);
        }

        public void Apply(LimitFilter filter) => _limit = filter.Value;
        public void Apply(MasterdataTypeFilter filter) => _query = _query.Where($"md.type = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MasterdataNameFilter filter) => _query = _query.Where($"EXISTS(SELECT attr.id FROM cbv.attribute attr WHERE attr.masterdata_id = md.id AND attr.masterdata_type = md.type AND attr.id = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(MasterdataExistsAttibuteFilter filter) => _query = _query.Where($"md.id = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MasterdataDescendentNameFilter filter) => _query = _query.Where($"(md.id = ANY({_parameters.Add(filter.Values)}) OR EXISTS(SELECT h.parent_id FROM cbv.hierarchy h WHERE h.children_id = md.id AND h.type = md.type AND h.parent_id = ANY({_parameters.Last})))");

        public async Task<IEnumerable<EpcisMasterData>> Fetch(string[] attributes, bool includeChildren, CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var masterData = await _connection.QueryAsync<EpcisMasterData>(new CommandDefinition(_sqlTemplate.RawSql, _parameters.Values, cancellationToken: cancellationToken));

            if (attributes != null)
            {
                var query = !attributes.Any() ? PgSqlMasterdataRequests.AllAttributeQuery : PgSqlMasterdataRequests.AttributeQuery;
                var relatedAttribute = await _connection.QueryAsync<MasterDataAttribute>(new CommandDefinition(query, new { Ids = masterData.Select(x => x.Id).ToArray(), Attributes = attributes }, cancellationToken: cancellationToken));
                masterData.ForEach(m => m.Attributes.AddRange(relatedAttribute.Where(a => a.ParentId == m.Id && a.ParentType == m.Type)));
            }
            if (includeChildren)
            {
                var children = await _connection.QueryAsync<EpcisMasterDataHierarchy>(new CommandDefinition(PgSqlMasterdataRequests.ChildrenQuery, new { Ids = masterData.Select(x => x.Id).ToArray() }, cancellationToken: cancellationToken));
                masterData.ForEach(m => m.Children.AddRange(children.Where(c => c.ParentId == m.Id && c.Type == m.Type)));
            }

            return masterData;
        }

        private IList<MasterDataField> CreateHierarchy(IEnumerable<MasterDataField> fields, int? parentId = null)
        {
            var elements = fields.Where(x => x.InternalParentId == parentId);
            elements.ForEach(x => x.Children = CreateHierarchy(fields, x.Id));

            return elements.ToList();
        }
    }
}
