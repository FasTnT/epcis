using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model;
using System.Threading;

namespace FasTnT.Host
{
    internal class EpcisCaptureMiddleware : EpcisMiddleware<Request>
    {
        public EpcisCaptureMiddleware(RequestDelegate next, string path) : base(next, path) { }

        public override async Task Process(Request request, CancellationToken cancellationToken)
        {
            switch (request)
            {
                case CaptureRequest captureRequest:
                    await Execute<CaptureService>(async s => await s.Capture(captureRequest, cancellationToken));
                    break;
                case EpcisQueryCallbackDocument queryCallbackDocument:
                    await Execute<CallbackService>(async s => await s.Process(queryCallbackDocument, cancellationToken));
                    break;
                case EpcisQueryCallbackException queryCallbackException:
                    await Execute<CallbackService>(async s => await s.ProcessException(queryCallbackException, cancellationToken));
                    break;
            }
        }
    }
}
