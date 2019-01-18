using FasTnT.Domain.Persistence;
using FasTnT.Model;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class CaptureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Capture(EpcisEventDocument events) => await CommitOnDispose(async () => await _unitOfWork.EventStore.Store(events));
        public async Task Capture(EpcisMasterdataDocument masterData) => await CommitOnDispose(async () => await _unitOfWork.MasterDataManager.Store(masterData));

        private async Task CommitOnDispose(Func<Task> method)
        {
            using(new CommitOnDispose(_unitOfWork))
            {
                await method();
            }
        }
    }
}
