using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Capture;
using FasTnT.Parsers.Xml.Formatters;
using FasTnT.Parsers.Xml.Parsers.Query;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Parsers.Xml
{
    public class XmlCommandFormatter : ICommandFormatter
    {
        public string ContentType => "application/xml";

        public async Task<ICaptureRequest> ParseCapture(Stream input, CancellationToken cancellationToken)
        {
            return await XmlRequestParser.Read(input, cancellationToken);
        }

        public Task<IQueryRequest> ParseQuery(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();

        public async Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken cancellationToken)
        {
            var formatter = new XmlResponseFormatter();

            await formatter.Write(epcisResponse, body, cancellationToken);
        }
    }
}
