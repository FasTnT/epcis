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
            var document = await XmlDocumentParser.Instance.Parse(input, cancellationToken);

            if (document.Root.Name == XName.Get("EPCISDocument", EpcisNamespaces.Capture))
            {
                return new CaptureEpcisDocumentRequest
                {
                    Request = ParseRequest(document.Root)
                };
            }
            else if (document.Root.Name == XName.Get("EPCISQueryDocument", EpcisNamespaces.Query)) // Subscription result
            {
                return ParseCallback(document);
            }
            else if (document.Root.Name == XName.Get("EPCISMasterDataDocument", EpcisNamespaces.MasterData))
            {
                return new CaptureEpcisDocumentRequest
                {
                    Request = ParseRequest(document.Root),
                };
            }

            throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }

        private ICaptureRequest ParseCallback(XDocument document)
        {
            var callbackType = document.Root.Element("EPCISBody").Elements().First().Name.LocalName;

            switch (callbackType)
            {
                case "QueryTooLargeException":
                case "ImplementationException":
                    return new CaptureEpcisExceptionRequest
                    {
                        Header = ParseRequest(document.Root),
                        SubscriptionName = document.Root.Element("EPCISBody").Element(XName.Get(callbackType, EpcisNamespaces.Query)).Element("subscriptionID").Value,
                        Reason = document.Root.Element("EPCISBody").Element(XName.Get(callbackType, EpcisNamespaces.Query)).Element("reason").Value,
                        CallbackType = Enumeration.GetByDisplayNameInvariant<QueryCallbackType>(callbackType)
                    };
                case "QueryResults":
                    return new CaptureEpcisQueryCallbackRequest
                    {
                        Header = ParseRequest(document.Root),
                        SubscriptionName = document.Root.Element("EPCISBody").Element(XName.Get("QueryResults", EpcisNamespaces.Query)).Element("subscriptionID").Value
                    };
            }

            throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
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
    }
}
