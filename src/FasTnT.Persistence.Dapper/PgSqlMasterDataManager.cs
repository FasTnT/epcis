using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using MoreLinq;
using static Dapper.SqlBuilder;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlMasterDataManager : IMasterDataManager
    {
        private readonly DapperUnitOfWork _unitOfWork;
        private SqlBuilder _query = new SqlBuilder();
        private Template _sqlTemplate;
        private QueryParameters _parameters = new QueryParameters();

        private int _limit = 0;

        public PgSqlMasterDataManager(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sqlTemplate = _query.AddTemplate(SqlRequests.MasterDataQuery);
        }

        public async Task Store(EpcisMasterdataDocument masterDataDocument)
        {
            foreach (var masterData in masterDataDocument.MasterDataList)
            {
                await _unitOfWork.Execute(SqlRequests.MasterDataDelete, masterData);
                await _unitOfWork.Execute(SqlRequests.MasterDataInsert, masterData);
                foreach (var attribute in masterData.Attributes)
                {
                    await _unitOfWork.Execute(SqlRequests.MasterDataAttributeInsert, attribute);
                    await _unitOfWork.Execute(SqlRequests.MasterDataAttributeFieldInsert, attribute.Fields);
                }
            }

            var hierarchies = masterDataDocument.MasterDataList.SelectMany(x => x.Children.Select(c => new EpcisMasterDataHierarchy { Type = x.Type, ChildrenId = c.ChildrenId, ParentId = x.Id }));
            await _unitOfWork.Execute(SqlRequests.MasterDataHierarchyInsert, hierarchies);
        }

        public void Limit(int limit) => _limit = limit;
        public void WhereAnyAttributeNamed(string[] values) => _query = _query.Where($"EXISTS(SELECT attr.id FROM cbv.attribute attr WHERE attr.masterdata_id = md.id AND attr.masterdata_type = md.type AND attr.id = ANY({_parameters.Add(values)}))");
        public void WhereIdIn(string[] values) => _query = _query.Where($"md.id = ANY({_parameters.Add(values)})");
        public void WhereIsDescendantOf(string[] values) => _query = _query.Where($"(md.id = ANY({_parameters.Add(values)}) OR EXISTS(SELECT h.parent_id FROM cbv.hierarchy h WHERE h.children_id = md.id AND h.type = md.type AND h.parent_id = ANY({_parameters.Last})))");
        public void WhereTypeIn(string[] values) => _query = _query.Where($"md.type = ANY({_parameters.Add(values)})");

        public async Task<IEnumerable<EpcisMasterData>> ToList(string[] attributes, bool includeChildren)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var masterData = await _unitOfWork.Query<EpcisMasterData>(_sqlTemplate.RawSql, _parameters.Values);

            if (attributes != null)
            {
                var query = !attributes.Any() ? SqlRequests.MasterDataAllAttributeQuery : SqlRequests.MasterDataAttributeQuery;
                var relatedAttribute = await _unitOfWork.Query<MasterDataAttribute>(query, new { Ids = masterData.Select(x => x.Id).ToArray(), Attributes = attributes });
                masterData.ForEach(m => m.Attributes.AddRange(relatedAttribute.Where(a => a.ParentId == m.Id && a.ParentType == m.Type)));
            }
            if (includeChildren)
            {
                var children = await _unitOfWork.Query<EpcisMasterDataHierarchy>("SELECT type, parent_id, children_id FROM cbv.hierarchy WHERE parent_id = ANY(@Ids);", new { Ids = masterData.Select(x => x.Id).ToArray() });
                masterData.ForEach(m => m.Children.AddRange(children.Where(c => c.ParentId == m.Id && c.Type == m.Type)));
            }

            return masterData;
        }
    }
}
