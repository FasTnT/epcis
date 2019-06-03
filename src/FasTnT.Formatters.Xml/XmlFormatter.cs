using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Xml
{
    public class XmlFormatter : IFormatter
    {
        private XmlFormatter() { }
        public static IFormatter Instance = new XmlFormatter();

        public string ContentType => "application/xml";

        public Task<Request> ReadRequest(Stream input, CancellationToken cancellationToken) => new XmlRequestFormatter().Read(input, cancellationToken);
        public Task WriteRequest(Request entity, Stream output, CancellationToken cancellationToken) => new XmlRequestFormatter().Write(entity, output, cancellationToken);

        public Task<EpcisQuery> ReadQuery(Stream input, CancellationToken cancellationToken) => new XmlQueryFormatter().Read(input, cancellationToken);
        public Task WriteQuery(EpcisQuery entity, Stream output, CancellationToken cancellationToken) => new XmlQueryFormatter().Write(entity, output, cancellationToken);

        public Task<IEpcisResponse> ReadResponse(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteResponse(IEpcisResponse entity, Stream output, CancellationToken cancellationToken) => new XmlResponseFormatter().Write(entity, output, cancellationToken);
    }
}
