using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentRequest : ICaptureRequest
    {
        public EpcisRequestHeader Header { get; set; }
        public IList<EpcisEvent> EventList { get; set; } = new List<EpcisEvent>();
        public IList<EpcisMasterData> MasterDataList { get; set; } = new List<EpcisMasterData>();

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
                    Header = request.Header,
                    EventList = request.EventList,
                    MasterdataList = request.MasterDataList
                };

                await _documentStore.Capture(captureRequest, _context, cancellationToken);

                return EmptyResponse.Value;
            }
        }
    }
}
