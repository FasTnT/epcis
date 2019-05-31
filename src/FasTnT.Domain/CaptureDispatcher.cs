using FasTnT.Domain.Services;
using FasTnT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain
{
    public class CaptureDispatcher
    {
        private readonly CaptureService _service;

        public CaptureDispatcher(CaptureService service) => _service = service;

        public async Task DispatchDocument(Request document, CancellationToken cancellationToken)
        {
            switch (document)
            {
                case CaptureRequest request:
                    await _service.CaptureDocument(request, cancellationToken); break;
                case EpcisQueryCallbackDocument request:
                    await _service.CaptureCallback(request, cancellationToken); break;
                case EpcisQueryCallbackException request:
                    await _service.CaptureCallbackException(request, cancellationToken); break;
                default:
                    throw new System.ArgumentOutOfRangeException($"Unable to process Query of type '{document.GetType().Name}'");
            }
        }
    }
}
