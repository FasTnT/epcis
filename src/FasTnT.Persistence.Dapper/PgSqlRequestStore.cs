using System;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlRequestStore : IRequestStore
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlRequestStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Guid> Store(EpcisRequestHeader request)
        {
            var epcisRequest = ModelMapper.Map<EpcisRequestHeader, RequestHeaderEntity>(request, r => r.Id = Guid.NewGuid());
            await _unitOfWork.Query<Guid>(SqlRequests.StoreRequest, epcisRequest);

            return epcisRequest.Id;
        }
    }
}
