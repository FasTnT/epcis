using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Json
{
    public class JsonFormatter : IFormatter
    {
        public static IFormatter Instance = new JsonFormatter();

        public string ContentType => "application/json";
        public Task<EpcisQuery> ReadQuery(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Request> ReadRequest(Stream input, CancellationToken cancellationToken) => new JsonRequestFormatter().Read(input, cancellationToken);
        public Task<IEpcisResponse> ReadResponse(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteQuery(EpcisQuery entity, Stream output, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteRequest(Request entity, Stream output, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteResponse(IEpcisResponse entity, Stream output, CancellationToken cancellationToken) => new JsonResponseFormatter().Write(entity, output, cancellationToken);
    }
}
