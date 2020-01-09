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
        private ITransactionProvider _transactionProvider;
        private readonly RequestContext _context;
        private readonly IDocumentStore _documentStore;

        public CaptureEpcisDocumentHandler(ITransactionProvider transactionProvider, RequestContext context, IDocumentStore documentStore)
        {
            _transactionProvider = transactionProvider;
            _context = context;
            _documentStore = documentStore;
        }

        public async Task<IEpcisResponse> Handle(CaptureEpcisDocumentRequest request, CancellationToken cancellationToken)
        {
            using (var tx = _transactionProvider.BeginTransaction())
            {
                var captureRequest = new CaptureDocumentRequest
                {
                    Payload = request,
                    User = _context.User,
                    Transaction = tx,
                    CancellationToken = cancellationToken
                };

                await _documentStore.Capture(captureRequest);
                tx.Commit();
            }

            return EmptyResponse.Value;
        }
    }
}
