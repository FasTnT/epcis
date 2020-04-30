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
        public async Task<ICaptureRequest> Read(Stream input, CancellationToken cancellationToken)
        {
            var request = default(EpcisRequest);
            var document = await XmlDocumentParser.Instance.Parse(input, cancellationToken);

            if (document.Root.Name == XName.Get("EPCISDocument", EpcisNamespaces.Capture))
            {
                request = ParseRequest(document.Root);
            }
            else if (document.Root.Name == XName.Get("EPCISQueryDocument", EpcisNamespaces.Query)) // Subscription result
            {
                request = ParseCallback(document);
            }
            else if (document.Root.Name == XName.Get("EPCISMasterDataDocument", EpcisNamespaces.MasterData))
            {
                return new CaptureEpcisDocumentRequest
                {
                    Request = ParseRequest(document.Root),
                };
            }

            return request != default 
                    ? new CaptureEpcisDocumentRequest {  Request = request }
                    : throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }

        private EpcisRequest ParseCallback(XDocument document)
        {
            var request = default(EpcisRequest);
            var callback = document.Root.Element("EPCISBody").Elements().First();
            var callbackType = callback.Name.LocalName;

            switch (callbackType)
            {
                case "QueryTooLargeException":
                case "ImplementationException":
                case "QueryResults":
                    request = ParseRequest(document.Root);
                    request.SubscriptionCallback = ParseCallback(callback);
                    break;
            }

            return request ?? throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }

        private EpcisRequest ParseRequest(XElement root)
        {
            return new EpcisRequest
            {
                StandardBusinessHeader = XmlHeaderParser.Parse(root.XPathSelectElement("EPCISHeader/sbdh:StandardBusinessDocumentHeader", EpcisNamespaces.Manager)),
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                SchemaVersion = root.Attribute("schemaVersion").Value,
                CustomFields = XmlCustomFieldParser.ParseCustomFields(root.XPathSelectElement("EPCISHeader"), FieldType.HeaderExtension),
                EventList = XmlEventsParser.ParseEvents(root.XPathSelectElement("EPCISBody/EventList")?.Elements()?.ToArray() ?? Array.Empty<XElement>()),
                MasterdataList = XmlMasterDataParser.ParseMasterDatas(root.Element("EPCISBody")?.Element("VocabularyList")?.Elements("Vocabulary") ?? Array.Empty<XElement>())
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
