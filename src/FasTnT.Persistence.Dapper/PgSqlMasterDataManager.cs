using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Model.MasterDatas;
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

        public async Task Store(EpcisMasterData masterData)
        {
            await _unitOfWork.Execute(SqlRequests.MasterDataInsert, masterData);
            foreach (var attribute in masterData.Attributes) await _unitOfWork.Execute(SqlRequests.MasterDataAttributeInsert, attribute);
        }

        public void Limit(int limit) => _limit = limit;
        public void WhereAnyAttributeNamed(string[] values) => _query = _query.Where($"EXISTS(SELECT attr.id FROM cbv.attribute attr WHERE attr.parent_id = md.id AND attr.parent_type = md.type AND attr.id = ANY({_parameters.Add(values)}))");
        public void WhereIdIn(string[] values) => _query = _query.Where($"md.id = ANY({_parameters.Add(values)})");
        public void WhereTypeIn(string[] values) => _query = _query.Where($"md.type = ANY({_parameters.Add(values)})");

        public async Task<IEnumerable<EpcisMasterData>> ToList(bool includeAttributes)
        {
            _parameters.SetLimit(_limit > 0 ? _limit : int.MaxValue);
            var masterData = await _unitOfWork.Query<EpcisMasterData>(_sqlTemplate.RawSql, _parameters.Values);

            // TODO: handle includeAttributes

            return masterData;
        }
    }
}
