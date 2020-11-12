using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentHandler : IRequestHandler<CaptureEpcisDocumentRequest, IEpcisResponse>
    {
        private readonly RequestContext _context;
        private readonly IEpcisRequestStore _documentStore;

        public CaptureEpcisDocumentHandler(RequestContext context, IEpcisRequestStore documentStore)
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
            //foreach (var epc in evt.Epcs)
            //{
            //    UriValidator.Validate(epc.Id);
            //}

            if (IsAddOrDeleteAggregation(evt) && !evt.Epcs.Any(x => x.Type == EpcType.ParentId)) // TCR-7 parentID is Populated for ADD or DELETE Actions in Aggregation Events
            {
                throw new EpcisException(ExceptionType.ValidationException, "TCR-7: parentID must be populated for ADD or DELETE aggregation event.");
            }
        }

        private static bool IsAddOrDeleteAggregation(EpcisEvent evt) => evt.Type == EventType.Aggregation && new[] { EventAction.Add, EventAction.Delete }.Contains(evt.Action);
    }
}
