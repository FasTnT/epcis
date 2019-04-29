using System;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using FasTnT.Model.Users;

// TODO: store StandardBusinessHeader if set
namespace FasTnT.Persistence.Dapper
{
    public class PgSqlRequestStore : IRequestStore
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlRequestStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Guid> Store(EpcisRequestHeader request, User user, CancellationToken cancellationToken)
        {
            var epcisRequest = ModelMapper.Map<EpcisRequestHeader, RequestHeaderEntity>(request, r => { r.Id = Guid.NewGuid(); r.UserId = user?.Id; });
            await _unitOfWork.Execute(SqlRequests.StoreRequest, epcisRequest, cancellationToken);

            return epcisRequest.Id;
        }
    }
}
