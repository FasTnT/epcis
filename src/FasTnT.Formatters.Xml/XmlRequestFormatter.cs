using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using FasTnT.Formatters.Xml.Parsers;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Formatters.Xml.Validation;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Utils;

namespace FasTnT.Formatters.Xml
{
    public class XmlRequestFormatter : XmlEpcisWriter<Request>
    {
        public async Task<Request> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.Load(input, cancellationToken);

            if (document.Root.Name == XName.Get("EPCISDocument", EpcisNamespaces.Capture))
            {
                return new CaptureRequest
                {
                    Header = ParseHeader(document.Root),
                    EventList = XmlEventsParser.ParseEvents(document.Root.XPathSelectElement("EPCISBody/EventList").Elements().ToArray()),
                    MasterDataList = XmlMasterDataParser.ParseMasterDatas(document.Root.XPathSelectElement("EPCISHeader/extension/EPCISMasterData/VocabularyList")?.Elements()?.ToArray() ?? new XElement[0]),
                };
            }
            else if (document.Root.Name == XName.Get("EPCISQueryDocument", EpcisNamespaces.Query)) // Subscription result
            {
                return ParseCallback(document);
            }
            else if (document.Root.Name == XName.Get("EPCISMasterDataDocument", EpcisNamespaces.MasterData))
            {
                return new CaptureRequest
                {
                    Header = ParseHeader(document.Root),
                    MasterDataList = XmlMasterDataParser.ParseMasterDatas(document.Root.Element("EPCISBody").Element("VocabularyList").Elements("Vocabulary"))
                };
            }

            throw new Exception($"Document with root '{document.Root.Name.ToString()}' is not expected here.");
        }

        public override async Task Write(Request entity, Stream output, CancellationToken cancellationToken)
        {
            await Write(entity, output, e => Write(e), cancellationToken);
        }

        private Request ParseCallback(XDocument document)
        {
            var callbackType = document.Root.Element("EPCISBody").Elements().First().Name.LocalName;

            switch (callbackType)
            {
                case "QueryTooLargeException":
                case "ImplementationException":
                    return new EpcisQueryCallbackException
                    {
                        Header = ParseHeader(document.Root),
                        SubscriptionName = document.Root.Element("EPCISBody").Element(XName.Get(callbackType, EpcisNamespaces.Query)).Element("subscriptionID").Value,
                        Reason = document.Root.Element("EPCISBody").Element(XName.Get(callbackType, EpcisNamespaces.Query)).Element("reason").Value,
                        CallbackType = Enumeration.GetByDisplayNameInvariant<QueryCallbackType>(callbackType)
                    };
                case "QueryResults":
                    return new EpcisQueryCallbackDocument
                    {
                        Header = ParseHeader(document.Root),
                        SubscriptionName = document.Root.Element("EPCISBody").Element(XName.Get("QueryResults", EpcisNamespaces.Query)).Element("subscriptionID").Value,
                        EventList = XmlEventsParser.ParseEvents(document.Root.Element("EPCISBody").Element(XName.Get("QueryResults", EpcisNamespaces.Query)).Element("resultsBody").Element("EventList").Elements().ToArray())
                    };
            }

            throw new Exception($"Document with root '{document.Root.Name.ToString()}' is not expected here.");
        }

        private EpcisRequestHeader ParseHeader(XElement root)
        {
            return new EpcisRequestHeader
            {
                StandardBusinessHeader = XmlHeaderParser.Parse(root.XPathSelectElement("EPCISHeader/sbdh:StandardBusinessDocumentHeader", EpcisNamespaces.Manager)),
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                SchemaVersion = root.Attribute("schemaVersion").Value,
                CustomFields = XmlCustomFieldParser.ParseCustomFields(root.XPathSelectElement("EPCISHeader"), FieldType.HeaderExtension)
            };
        }

        private XDocument Write(CaptureRequest entity)
        {
            return new XDocument(
                XName.Get("EPCISDocument", EpcisNamespaces.Capture),
                new XAttribute("creationDate", entity.Header.DocumentTime.ToString("yyyy-MM-ddThh:MM:ssZ")),
                new XAttribute("schemaVersion", entity.Header.SchemaVersion),
                new XElement("EPCISBody", new XElement("EventList", entity.EventList.Select(new XmlEventFormatter().Format)))
            );
        }
    }
}
