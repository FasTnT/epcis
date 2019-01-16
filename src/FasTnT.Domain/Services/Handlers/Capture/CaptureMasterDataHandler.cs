using FasTnT.Model.Responses;
using FasTnT.Domain.Persistence;
using System.Threading.Tasks;
using FasTnT.Model;
using System;

namespace FasTnT.Domain.Services.Handlers.Capture
{
    public class CaptureMasterDataHandler : IHandler<EpcisMasterdataDocument>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaptureMasterDataHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public Task<IEpcisResponse> Handle(EpcisMasterdataDocument request)
        {
            throw new NotImplementedException();
        }
    }
}
