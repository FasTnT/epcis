using FasTnT.Commands.Responses;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Commands
{
    public interface ICommandFormatter
    {
        string ContentType { get; }

        Task<IQueryRequest> ParseQuery(Stream input, CancellationToken cancellationToken);
        Task<ICaptureRequest> ParseCapture(Stream input, CancellationToken cancellationToken);
        Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken requestAborted);
    }
}
