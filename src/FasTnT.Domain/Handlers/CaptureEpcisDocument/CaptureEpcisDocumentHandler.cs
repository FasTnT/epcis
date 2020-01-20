using FasTnT.Commands.Requests;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;

namespace FasTnT.Domain.Handlers.CaptureEpcisDocument
{
    public class CaptureEpcisDocumentHandler : IRequestHandler<CaptureEpcisDocumentRequest, IEpcisResponse>
    {
        private readonly RequestContext _context;
        private readonly IDocumentStore _documentStore;

        public CaptureEpcisDocumentHandler(RequestContext context, IDocumentStore documentStore)
        {
            _context = context;
            _documentStore = documentStore;
        }

        public async Task<IEpcisResponse> Handle(CaptureEpcisDocumentRequest request, CancellationToken cancellationToken)
        {
            var captureRequest = new CaptureDocumentRequest
            {
                Payload = request,
                User = _context.User,
                CancellationToken = cancellationToken
            };

            await _documentStore.Capture(captureRequest);

            return EmptyResponse.Value;
        }
    }
}
