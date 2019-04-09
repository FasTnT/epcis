using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model;

namespace FasTnT.Host
{
    internal class EpcisCaptureMiddleware : EpcisMiddleware<Request>
    {
        public EpcisCaptureMiddleware(RequestDelegate next, string path) : base( next, path) { }

        public override async Task Process(Request request)
        {
            if (request is CaptureRequest eventDocument)
                await Execute<CaptureService>(async s => await s.Capture(eventDocument));
            else if (request is EpcisQueryCallbackDocument queryCallbackDocument)
                await Execute<CallbackService>(async s => await s.Process(queryCallbackDocument));
            else if (request is EpcisQueryCallbackException queryCallbackException)
                await Execute<CallbackService>(async s => await s.ProcessException(queryCallbackException));
        }
    }
}
