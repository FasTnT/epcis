using System.Threading;
using System.Threading.Tasks;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using FasTnT.Model.Enums;
using MediatR;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisExceptionRequest : ICaptureRequest
    {
        public EpcisRequest Header { get; set; }
        public QueryCallbackType CallbackType { get; set; }
        public string Severity { get; set; } = "ERROR";
        public string Reason { get; set; }
        public string SubscriptionName { get; set; }

        public class CaptureEpcisExceptionHandler : IRequestHandler<CaptureEpcisExceptionRequest, IEpcisResponse>
        {
            private readonly RequestContext _context;
            private readonly IDocumentStore _documentStore;

            public CaptureEpcisExceptionHandler(RequestContext context, IDocumentStore documentStore)
            {
                _context = context;
                _documentStore = documentStore;
            }

            public async Task<IEpcisResponse> Handle(CaptureEpcisExceptionRequest request, CancellationToken cancellationToken)
            {
                var captureRequest = new CaptureCallbackRequest
                {
                    SubscriptionId = request.SubscriptionName,
                    CallbackType = request.CallbackType,
                    Header = request.Header
                };

                await _documentStore.Capture(captureRequest, _context, cancellationToken);

                return EmptyResponse.Value;
            }
        }
    }
}
