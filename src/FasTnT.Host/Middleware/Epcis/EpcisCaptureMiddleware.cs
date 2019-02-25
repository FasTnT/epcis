using Microsoft.Extensions.Logging;
using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model;

namespace FasTnT.Host
{
    internal class EpcisCaptureMiddleware : EpcisMiddleware<Request>
    {
        public EpcisCaptureMiddleware(ILogger<EpcisCaptureMiddleware> logger, RequestDelegate next, string path)
            : base(logger, next, path) { }

        public override async Task Process(Request request)
        {
            if (request is EpcisEventDocument eventDocument)
                await Execute<CaptureService>(async s => await s.Capture(eventDocument));
            else if (request is EpcisMasterdataDocument masterDataDocument)
                await Execute<CaptureService>(async s => await s.Capture(masterDataDocument));
            else if (request is EpcisQueryCallbackDocument queryCallbackDocument)
                await Execute<CallbackService>(async s => await s.Process(queryCallbackDocument));
            else if (request is EpcisQueryCallbackException queryCallbackException)
                await Execute<CallbackService>(async s => await s.ProcessException(queryCallbackException));
        }
    }
}
