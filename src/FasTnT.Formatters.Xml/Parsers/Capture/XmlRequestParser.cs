using FasTnT.Commands.Requests;
using FasTnT.Domain.Commands;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Parsers.Xml.Capture
{
    public static class XmlRequestParser
    {
        public async static Task<ICaptureRequest> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.Parse(input, cancellationToken);
            var request = XmlEpcisDocumentParser.Parse(document.Root);

            return request != default
                    ? new CaptureEpcisDocumentRequest { Request = request }
                    : throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }
    }
}
