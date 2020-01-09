using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Capture;
using FasTnT.Parsers.Xml.Formatters;
using FasTnT.Parsers.Xml.Parsers.Query;
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
            var requestParser = new XmlRequestParser();

            return await requestParser.Read(input, cancellationToken);
        }

        public async Task<IQueryRequest> ParseQuery(Stream input, CancellationToken cancellationToken)
        {
            var queryParser = new XmlQueryParser();

            return await queryParser.Read(input, cancellationToken);
        }

        public async Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken cancellationToken)
        {
            var formatter = new XmlResponseFormatter();

            await formatter.Write(epcisResponse, body, cancellationToken);
        }
    }
}
