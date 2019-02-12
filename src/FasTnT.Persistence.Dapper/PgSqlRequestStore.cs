using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlRequestStore : IRequestStore
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlRequestStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(EpcisRequestHeader request)
        {
            await _unitOfWork.Execute(SqlRequests.StoreRequest, request);
        }
    }
}
