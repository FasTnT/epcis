using FasTnT.Domain.Persistence;
using System.Threading.Tasks;
using FasTnT.Model;

namespace FasTnT.Domain.Services.Handlers.Capture
{
    public class CaptureMasterDataHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureMasterDataHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Handle(EpcisMasterdataDocument request)
        {
            using (new CommitOnDispose(_unitOfWork))
            {
                foreach (var masterData in request.MasterDataList)
                {
                    await _unitOfWork.MasterDataManager.Store(masterData);
                }
            }
        }
    }
}
