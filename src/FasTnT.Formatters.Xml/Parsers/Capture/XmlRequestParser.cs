using FasTnT.Commands.Requests;
using FasTnT.Domain.Commands;
using FasTnT.Model;
using FasTnT.Model.Enums;
using FasTnT.Model.Utils;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FasTnT.Parsers.Xml.Capture
{
    public class XmlRequestParser
    {
        private const string QueryBodyPath = "EPCISBody/query:QueryResults/resultsBody";
        private const string CaptureBodyPath = "EPCISBody";

        public async Task<ICaptureRequest> Read(Stream input, CancellationToken cancellationToken)
        {
            EpcisRequest request;
            var document = await XmlDocumentParser.Instance.Parse(input, cancellationToken);

            if (document.Root.Name == XName.Get("EPCISQueryDocument", EpcisNamespaces.Query)) // Subscription callback result
            {
                request = ParseSubscriptionCallback(document);
            }
            else
            {
                request = ParseRequest(document.Root, CaptureBodyPath);
            }

            return request != default 
                    ? new CaptureEpcisDocumentRequest {  Request = request }
                    : throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }

        private EpcisRequest ParseSubscriptionCallback(XDocument document)
        {
            var callback = document.Root.Element("EPCISBody").Elements().First();
            var request = ParseRequest(document.Root, QueryBodyPath);

            request.SubscriptionCallback = ParseCallback(callback);

            return request ?? throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }

        private EpcisRequest ParseRequest(XElement root, string dataRootPath)
        {
            return new EpcisRequest
            {
                StandardBusinessHeader = XmlHeaderParser.Parse(root.XPathSelectElement("EPCISHeader/sbdh:StandardBusinessDocumentHeader", EpcisNamespaces.Manager)),
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                SchemaVersion = root.Attribute("schemaVersion").Value,
                // CustomFields = XmlCustomFieldParser.ParseCustomFields(root.XPathSelectElement("EPCISHeader"), FieldType.HeaderExtension),
                EventList = XmlEventsParser.ParseEvents(root.XPathSelectElement($"{dataRootPath}/EventList", EpcisNamespaces.Manager)?.Elements()?.ToArray() ?? Array.Empty<XElement>()),
                MasterdataList = XmlMasterDataParser.ParseMasterDatas(root.XPathSelectElement($"{dataRootPath}/VocabularyList", EpcisNamespaces.Manager)?.Elements("Vocabulary") ?? Array.Empty<XElement>())
            };
        }

        private SubscriptionCallback ParseCallback(XElement callbackElement)
        {
            return new SubscriptionCallback
            {
                SubscriptionId = callbackElement.Element("subscriptionID").Value,
                Reason = callbackElement.Element("reason")?.Value,
                CallbackType = Enumeration.GetByDisplayNameInvariant<QueryCallbackType>(callbackElement.Name.LocalName),
            };
        }
    }
}
