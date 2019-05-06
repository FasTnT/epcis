using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using FasTnT.Model.Events;
using FasTnT.Model.Users;

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
            await StoreStandardBusinessHeader(request, epcisRequest, cancellationToken);

            return epcisRequest.Id;
        }

        private async Task StoreStandardBusinessHeader(EpcisRequestHeader request, RequestHeaderEntity epcisRequest, CancellationToken cancellationToken)
        {
            if (request.StandardBusinessHeader == null) return;

            var header = ModelMapper.Map<StandardBusinessHeader, StandardBusinessHeaderEntity>(request.StandardBusinessHeader, r => r.Id = epcisRequest.Id);
            var contactInformations = request.StandardBusinessHeader.ContactInformations.Select((x, i) => ModelMapper.Map<ContactInformation, ContactInformationEntity>(x, r => { r.HeaderId = header.Id; r.Id = i; }));

            await _unitOfWork.Execute(SqlRequests.StoreStandardHeader, header, cancellationToken);
            await _unitOfWork.Execute(SqlRequests.StoreStandardHeaderContactInformations, contactInformations, cancellationToken);
        }
    }
}
