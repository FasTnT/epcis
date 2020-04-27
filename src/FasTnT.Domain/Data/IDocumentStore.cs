using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IDocumentStore
    {
        Task Capture(EpcisRequest captureRequest, RequestContext context, CancellationToken cancellationToken);
        Task Capture(CaptureCallbackRequest captureRequest, RequestContext context, CancellationToken cancellationToken);
    }
}
