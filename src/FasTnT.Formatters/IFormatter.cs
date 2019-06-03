using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatters
{
    public interface IFormatter
    {
        string ContentType { get; }

        Task<Request> ReadRequest(Stream input, CancellationToken cancellationToken);
        Task WriteRequest(Request entity, Stream output, CancellationToken cancellationToken);

        Task<EpcisQuery> ReadQuery(Stream input, CancellationToken cancellationToken);
        Task WriteQuery(EpcisQuery entity, Stream output, CancellationToken cancellationToken);

        Task<IEpcisResponse> ReadResponse(Stream input, CancellationToken cancellationToken);
        Task WriteResponse(IEpcisResponse entity, Stream output, CancellationToken cancellationToken);
    }
}
