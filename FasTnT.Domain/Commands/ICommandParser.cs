using FasTnT.Commands.Responses;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Commands
{
    public interface ICommandParser
    {
        string ContentType { get; }

        Task<IRequest<IEpcisResponse>> ParseCommand(Stream input, CancellationToken cancellationToken);
        Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken requestAborted);
    }
}
