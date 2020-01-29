using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using FasTnT.Model;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisQueryCallbackRequest : ICaptureRequest
    {
        public EpcisRequestHeader Header { get; set; }
        public EpcisEvent[] EventList { get; set; }
        public string SubscriptionName { get; set; }

        public class CaptureEpcisQueryCallbackHandler : IRequestHandler<CaptureEpcisQueryCallbackRequest, IEpcisResponse>
        {
            private readonly RequestContext _context;
            private readonly IDocumentStore _documentStore;

            public CaptureEpcisQueryCallbackHandler(RequestContext context, IDocumentStore documentStore)
            {
                _context = context;
                _documentStore = documentStore;
            }

            public Task<IEpcisResponse> Handle(CaptureEpcisQueryCallbackRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
