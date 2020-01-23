using FasTnT.Domain.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IDocumentStore
    {
        Task Capture(CaptureDocumentRequest captureRequest, RequestContext context, CancellationToken cancellationToken);
    }
}
