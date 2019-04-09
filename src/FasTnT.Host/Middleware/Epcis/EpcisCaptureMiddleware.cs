using Microsoft.Extensions.Logging;
using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model;
using System.Threading;

namespace FasTnT.Host
{
    internal class EpcisCaptureMiddleware : EpcisMiddleware<Request>
    {
        public EpcisCaptureMiddleware(RequestDelegate next, string path)
            : base(next, path) { }

        public override async Task Process(Request request, CancellationToken cancellationToken)
        {
            if (request is CaptureRequest eventDocument)
                await Execute<CaptureService>(async s => await s.Capture(eventDocument, cancellationToken));
            else if (request is EpcisQueryCallbackDocument queryCallbackDocument)
                await Execute<CallbackService>(async s => await s.Process(queryCallbackDocument, cancellationToken));
            else if (request is EpcisQueryCallbackException queryCallbackException)
                await Execute<CallbackService>(async s => await s.ProcessException(queryCallbackException, cancellationToken));
        }
    }
}
