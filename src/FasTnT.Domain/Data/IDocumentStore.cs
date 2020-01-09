using FasTnT.Domain.Data.Model;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IDocumentStore
    {
        Task Capture(CaptureDocumentRequest request);
    }
}
