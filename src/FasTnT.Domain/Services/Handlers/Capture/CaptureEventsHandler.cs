using FasTnT.Model.Responses;
using FasTnT.Domain.Persistence;
using System.Threading.Tasks;
using FasTnT.Model;

namespace FasTnT.Domain.Services.Handlers.Capture
{
    public class CaptureEventsHandler : IHandler<EpcisEventDocument>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureEventsHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEpcisResponse> Handle(EpcisEventDocument request)
        {
            using (new CommitOnDispose(_unitOfWork))
            {
                await _unitOfWork.EventStore.Store(request);

                return default(IEpcisResponse);
            }
        }
    }
}
