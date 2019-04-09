using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Model.MasterDatas;
using MoreLinq;
using static Dapper.SqlBuilder;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlMasterDataManager : IMasterDataManager
    {
        private SqlBuilder _query = new SqlBuilder();
        private readonly DapperUnitOfWork _unitOfWork;
        private readonly Template _sqlTemplate;
        private readonly QueryParameters _parameters = new QueryParameters();

        private int _limit = 0;

        public PgSqlMasterDataManager(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sqlTemplate = _query.AddTemplate(SqlRequests.MasterDataQuery);
        }

        public async Task Store(Guid requestId, IEnumerable<EpcisMasterData> masterDataList, CancellationToken cancellationToken)
        {
            foreach (var masterData in masterDataList)
            {
                await _unitOfWork.Execute(SqlRequests.MasterDataDelete, masterData, cancellationToken);
                await _unitOfWork.Execute(SqlRequests.MasterDataInsert, masterData, cancellationToken);

                foreach (var attribute in masterData.Attributes)
                {
                    var output = new List<MasterDataFieldEntity>();
                    ParseFields(attribute.Fields, output);

                    await _unitOfWork.Execute(SqlRequests.MasterDataAttributeInsert, attribute, cancellationToken);
                    await _unitOfWork.Execute(SqlRequests.MasterDataAttributeFieldInsert, output, cancellationToken);
                }
            }

            var hierarchies = masterDataList.SelectMany(x => x.Children.Select(c => new EpcisMasterDataHierarchy { Type = x.Type, ChildrenId = c.ChildrenId, ParentId = x.Id }));
            await _unitOfWork.Execute(SqlRequests.MasterDataHierarchyInsert, hierarchies, cancellationToken);
        }

        private void ParseFields(IEnumerable<MasterDataField> fields, List<MasterDataFieldEntity> output, int? parentId = null)
        {
            foreach(var field in fields ?? new MasterDataField[0])
            {
                output.Add(field.Map<MasterDataField, MasterDataFieldEntity>(e => { e.Id = output.Count; e.InternalParentId = parentId; }));
                ParseFields(field.Children, output, output.Count - 1);
            }
        }

        public void Limit(int limit) => _limit = limit;
        public void WhereAnyAttributeNamed(string[] values) => _query = _query.Where($"EXISTS(SELECT attr.id FROM cbv.attribute attr WHERE attr.masterdata_id = md.id AND attr.masterdata_type = md.type AND attr.id = ANY({_parameters.Add(values)}))");
        public void WhereIdIn(string[] values) => _query = _query.Where($"md.id = ANY({_parameters.Add(values)})");
        public void WhereIsDescendantOf(string[] values) => _query = _query.Where($"(md.id = ANY({_parameters.Add(values)}) OR EXISTS(SELECT h.parent_id FROM cbv.hierarchy h WHERE h.children_id = md.id AND h.type = md.type AND h.parent_id = ANY({_parameters.Last})))");
        public void WhereTypeIn(string[] values) => _query = _query.Where($"md.type = ANY({_parameters.Add(values)})");

        public async Task<IEnumerable<EpcisMasterData>> ToList(string[] attributes, bool includeChildren, CancellationToken cancellationToken)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var masterData = await _unitOfWork.Query<EpcisMasterData>(_sqlTemplate.RawSql, _parameters.Values, cancellationToken);

            if (attributes != null)
            {
                var query = !attributes.Any() ? SqlRequests.MasterDataAllAttributeQuery : SqlRequests.MasterDataAttributeQuery;
                var relatedAttribute = await _unitOfWork.Query<MasterDataAttribute>(query, new { Ids = masterData.Select(x => x.Id).ToArray(), Attributes = attributes }, cancellationToken);
                masterData.ForEach(m => m.Attributes.AddRange(relatedAttribute.Where(a => a.ParentId == m.Id && a.ParentType == m.Type)));
            }
            if (includeChildren)
            {
                var children = await _unitOfWork.Query<EpcisMasterDataHierarchy>("SELECT type, parent_id, children_id FROM cbv.hierarchy WHERE parent_id = ANY(@Ids);", new { Ids = masterData.Select(x => x.Id).ToArray() }, cancellationToken);
                masterData.ForEach(m => m.Children.AddRange(children.Where(c => c.ParentId == m.Id && c.Type == m.Type)));
            }

            return masterData;
        }

        private IList<MasterDataField> CreateHierarchy(IEnumerable<MasterDataFieldEntity> fields, int? parentId = null)
        {
            var elements = fields.Where(x => x.InternalParentId == parentId);
            elements.ForEach(x => x.Children = CreateHierarchy(fields, x.Id));

            return elements.ToList<MasterDataField>();
        }
    }
}
