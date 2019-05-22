using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model;
using System.Threading;

namespace FasTnT.Host
{
    internal class EpcisCaptureMiddleware : EpcisMiddleware<Request>
    {
        public EpcisCaptureMiddleware(RequestDelegate next, string path) : base( next, path) { }

        public override async Task Process(Request request, CancellationToken cancellationToken)
        {
            switch (request)
            {
                case CaptureRequest captureRequest:
                    await Execute<CaptureService>(async s => await s.CaptureDocument(captureRequest, cancellationToken));
                    break;
                case EpcisQueryCallbackDocument queryCallbackDocument:
                    await Execute<CaptureService>(async s => await s.CaptureCallback(queryCallbackDocument, cancellationToken));
                    break;
                case EpcisQueryCallbackException queryCallbackException:
                    await Execute<CaptureService>(async s => await s.CaptureCallbackException(queryCallbackException, cancellationToken));
                    break;
            }
        }
    }
}
