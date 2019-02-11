using FasTnT.Domain.Extensions;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using MoreLinq;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CaptureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Capture(EpcisEventDocument captureDocument)
        {
            captureDocument.EventList.ForEach(x => x.Epcs.ForEach(e => UriValidator.Validate(e.Id)));

            await _unitOfWork.Execute(async tx =>
            {
                await tx.RequestStore.Store(captureDocument.Header);
                await tx.EventStore.Store(captureDocument.Header.Id, captureDocument.EventList);
            });
        }

        public async Task Capture(EpcisMasterdataDocument masterData) => await _unitOfWork.Execute(async tx => await tx.MasterDataManager.Store(masterData));
    }
}
