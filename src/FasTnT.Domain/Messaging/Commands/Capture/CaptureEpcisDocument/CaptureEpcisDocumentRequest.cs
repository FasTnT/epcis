using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using FasTnT.Model;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentRequest : ICaptureRequest
    {
        public EpcisRequest Request { get; set; }

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
                request.Request.EventList.ForEach(Validate);

                await _documentStore.Capture(request.Request, _context, cancellationToken);

                return EmptyResponse.Value;
            }

            internal static void Validate(EpcisEvent evt)
            {
                evt.Epcs.ForEach(e => UriValidator.Validate(e.Id));

                if (IsAddOrDeleteAggregation(evt) && !evt.Epcs.Any(x => x.Type == EpcType.ParentId)) // TCR-7 parentID is Populated for ADD or DELETE Actions in Aggregation Events
                {
                    throw new EpcisException(ExceptionType.ValidationException, "TCR-7: parentID must be populated for ADD or DELETE aggregation event.");
                }
            }

            private static bool IsAddOrDeleteAggregation(EpcisEvent evt) => evt.Type == EventType.Aggregation && new[] { EventAction.Add, EventAction.Delete }.Contains(evt.Action);
        }
    }
}
