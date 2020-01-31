using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Formatters;
using FasTnT.Parsers.Xml.Parsers.Query;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Parsers.Xml
{
    public class SoapCommandFormatter : ICommandFormatter
    {
        public string ContentType => "application/soap+xml";
        public Task<ICaptureRequest> ParseCapture(Stream input, CancellationToken cancellationToken) => throw new NotImplementedException();

        public async Task<IQueryRequest> ParseQuery(Stream input, CancellationToken cancellationToken)
        {
            var queryParser = new SoapQueryParser();

            return await queryParser.Read(input, cancellationToken);
        }

        public async Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken cancellationToken)
        {
            var formatter = new SoapResponseFormatter();

            await formatter.Write(epcisResponse, body, cancellationToken);
        }
    }
}
