using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Parsers.Query
{
    public class SoapQueryParser : XmlQueryParser
    {
        static readonly string SoapEnvelopNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        public override async Task<IQueryRequest> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
            var body = document.Element(XName.Get("Envelope", SoapEnvelopNamespace))?.Element(XName.Get("Body", SoapEnvelopNamespace));

            if (body == null || !body.HasElements)
            {
                throw new Exception("Malformed SOAP request");
            }
            else
            {
                return DispatchElement(body.Elements().SingleOrDefault(x => x.Name.NamespaceName == EpcisNamespaces.Query));
            }
        }
    }
}
