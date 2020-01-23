using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.QueryCallback
{
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
