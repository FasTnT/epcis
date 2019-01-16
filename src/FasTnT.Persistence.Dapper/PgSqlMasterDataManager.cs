using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlMasterDataManager : IMasterDataManager
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlMasterDataManager(DapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Store(EpcisMasterData masterData)
        {
            await _unitOfWork.Execute(SqlRequests.MasterDataInsert, masterData);
            foreach(var attribute in masterData.Attributes) await _unitOfWork.Execute(SqlRequests.MasterDataAttributeInsert, attribute);
        }
    }
}
