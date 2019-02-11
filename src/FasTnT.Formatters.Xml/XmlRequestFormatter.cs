using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Formatters.Xml.Validation;
using FasTnT.Model;

namespace FasTnT.Formatters.Xml
{
    public class XmlRequestFormatter : IRequestFormatter
    {
        public Request Read(Stream input)
        {
            var eventParser = new XmlEventsParser();
            var masterdataParser = new XmlMasterDataParser();
            var document = XmlDocumentParser.Instance.Load(input);

            if (document.Root.Name == XName.Get("EPCISDocument", EpcisNamespaces.Capture))
            {
                return new EpcisEventDocument
                {
                    CreationDate = DateTime.Parse(document.Root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                    SchemaVersion = document.Root.Attribute("schemaVersion").Value,
                    EventList = eventParser.ParseEvents(document.Root.XPathSelectElement("EPCISBody/EventList").Elements().ToArray())
                };
            }
            else if (document.Root.Name == XName.Get("EPCISQueryDocument", EpcisNamespaces.Query)) // Subscription result
            {
                return new EpcisQueryCallbackDocument
                {
                    CreationDate = DateTime.Parse(document.Root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                    SchemaVersion = document.Root.Attribute("schemaVersion").Value,
                    EventList = eventParser.ParseEvents(document.Root.Element("EPCISBody").Element(XName.Get("QueryResults", EpcisNamespaces.Query)).Element("resultsBody").Element("EventList").Elements().ToArray())
                };
            }
            else if (document.Root.Name == XName.Get("EPCISMasterDataDocument", EpcisNamespaces.MasterData))
            {
                return new EpcisMasterdataDocument
                {
                    CreationDate = DateTime.Parse(document.Root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                    SchemaVersion = document.Root.Attribute("schemaVersion").Value,
                    MasterDataList = masterdataParser.ParseMasterDatas(document.Root.Element("EPCISBody").Element("VocabularyList").Elements("Vocabulary"))
                };
            }

            throw new Exception($"Document with root '{document.Root.Name.ToString()}' is not expected here.");
        }

        public void Write(Request entity, Stream output)
        {
            XDocument document = Write((dynamic)entity);
            var bytes = Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces));

            output.Write(bytes, 0, bytes.Length);
        }

        private XDocument Write(EpcisEventDocument entity)
        {
            return new XDocument(
                XName.Get("EPCISDocument", EpcisNamespaces.Capture),
                new XAttribute("creationDate", entity.CreationDate.ToString("yyyy-MM-ddThh:MM:ssZ")),
                new XAttribute("schemaVersion", entity.SchemaVersion),
                new XElement("EPCISBody", new XElement("EventList", entity.EventList.Select(XmlEventFormatter.Format)))
            );
        }
    }
}
