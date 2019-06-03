using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatters.Xml
{
    public class SoapFormatter : IFormatter
    {
        private SoapFormatter() { }
        public static IFormatter Instance = new SoapFormatter();

        public string ContentType => "text/xml";

        public Task<Request> ReadRequest(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteRequest(Request entity, Stream output, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<EpcisQuery> ReadQuery(Stream input, CancellationToken cancellationToken) => new SoapQueryFormatter().Read(input, cancellationToken);
        public Task WriteQuery(EpcisQuery entity, Stream output, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<IEpcisResponse> ReadResponse(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task WriteResponse(IEpcisResponse entity, Stream output, CancellationToken cancellationToken) => new SoapResponseFormatter().Write(entity, output, cancellationToken);
    }
}
