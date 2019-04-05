using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using MoreLinq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CaptureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Capture(CaptureRequest captureDocument)
        {
            captureDocument.EventList.ForEach(EpcisEventValidator.Validate);

            await _unitOfWork.Execute(async tx =>
            {
                var headerId = await tx.RequestStore.Store(captureDocument.Header);

                await tx.MasterDataManager.Store(headerId, captureDocument.MasterDataList);
                await tx.EventStore.Store(headerId, captureDocument.EventList);
            });
        }
    }
}
