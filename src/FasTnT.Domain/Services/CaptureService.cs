using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Users;
using FasTnT.Model;
using MoreLinq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CaptureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserContext _userContext;

        public CaptureService(IUnitOfWork unitOfWork, UserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task Capture(CaptureRequest captureDocument, CancellationToken cancellationToken)
        {
            captureDocument.EventList.ForEach(EpcisEventValidator.Validate);

            await _unitOfWork.Execute(async tx =>
            {
                var headerId = await tx.RequestStore.Store(captureDocument.Header, _userContext.Current, cancellationToken);

                await tx.MasterDataManager.Store(headerId, captureDocument.MasterDataList, cancellationToken);
                await tx.EventStore.Store(headerId, captureDocument.EventList, cancellationToken);
            });
        }
    }
}
