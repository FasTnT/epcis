using Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlBuilder;

namespace FasTnT.Data.PostgreSql.Query
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
            _sqlTemplate = _query.AddTemplate(SqlQueries.Read_Masterdata);
        }

        public void Apply(LimitFilter filter) => _limit = filter.Value;
        public void Apply(MasterdataTypeFilter filter) => _query = _query.Where($"md.type = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MasterdataNameFilter filter) => _query = _query.Where($"EXISTS(SELECT attr.id FROM cbv.attribute attr WHERE attr.masterdata_id = md.id AND attr.masterdata_type = md.type AND attr.id = ANY({_parameters.Add(filter.Values)}))");
        public void Apply(MasterdataExistsAttibuteFilter filter) => _query = _query.Where($"md.id = ANY({_parameters.Add(filter.Values)})");
        public void Apply(MasterdataDescendentNameFilter filter) => _query = _query.Where($"(md.id = ANY({_parameters.Add(filter.Values)}) OR EXISTS(SELECT h.parent_id FROM cbv.hierarchy h WHERE h.children_id = md.id AND h.type = md.type AND h.parent_id = ANY({_parameters.Last})))");

        public async Task<IEnumerable<EpcisMasterData>> Fetch(string[] attributes, bool includeChildren, CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);

            var masterdataManager = new MasterdataDtoManager();

            masterdataManager.MasterDataDtos.AddRange(await _connection.QueryAsync<MasterDataDto>(new CommandDefinition(_sqlTemplate.RawSql, _parameters.Values, cancellationToken: cancellationToken)));

            if (attributes != null)
            {
                var query = !attributes.Any() ? SqlQueries.Read_MasterdataAllAttributes : SqlQueries.Read_MasterdataAttributes;
                var command = new CommandDefinition(query, new { Ids = masterdataManager.MasterDataDtos.Select(x => x.Id).ToArray(), Attributes = attributes }, cancellationToken: cancellationToken);
                masterdataManager.AttributeDtos.AddRange(await _connection.QueryAsync<MasterDataAttributeDto>(command));
            }
            if (includeChildren)
            {
                var command = new CommandDefinition(SqlQueries.Read_MasterdataChildren, new { Ids = masterdataManager.MasterDataDtos.Select(x => x.Id).ToArray() }, cancellationToken: cancellationToken);
                masterdataManager.HierarchyDtos.AddRange(await _connection.QueryAsync<MasterDataHierarchyDto>(command));
            }

            return masterdataManager.FormatMasterdata();
        }
    }
}
